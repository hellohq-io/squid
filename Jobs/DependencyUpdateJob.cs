using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.Extensions.Logging;

using MongoDB.Bson;

using Newtonsoft.Json;

using Q.Squid.Config;
using Q.Squid.DAL;
using Q.Squid.Models;
using Q.Squid.Store;
using Q.Squid.Utils;

namespace Q.Squid.Jobs
{
    public class DependencyUpdateJob : IJob
    {
        private const string BitbucketUrlTemplate = "https://api.bitbucket.org/2.0/repositories/{0}/{1}/src/{2}/{3}";
        private const string BitbucketUpdateUrlTemplate = "https://api.bitbucket.org/2.0/repositories/{0}/{1}/src/";
        private const string BitbucketPRUrlTemplate = "https://api.bitbucket.org/2.0/repositories/{0}/{1}/pullrequests/";

        private readonly ILogger _logger;
        private readonly HttpClient _client;

        public DependencyUpdateJob(
            ILogger<DependencyUpdateJob> logger,
            HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task Execute(
            RepositoryConfig repo,
            List<Update> updates,
            string reviewer)
        {
            _logger.LogTrace($"Updating dependencies of {repo.RepoSlug}");

            await UpdateDependencies(
                repo,
                updates,
                reviewer);
        }

        private async Task UpdateDependencies(
            RepositoryConfig repo,
            List<Update> updates,
            string reviewer)
        {
            var url = string.Format(
                BitbucketUrlTemplate,
                repo.TeamName,
                repo.RepoSlug,
                repo.Branch,
                repo.ProjectFile);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var password = Crypto.DecryptString(repo.Password, SquidConfig.CryptoKey);
            var auth = Base64.ToBase64(Encoding.UTF8, $"{repo.Username}:{password}");
            request.Headers.Add("Authorization", $"Basic {auth}");

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                XDocument projectFile = XDocument.Parse(content, LoadOptions.PreserveWhitespace);

                foreach (var dependency in updates)
                {
                    projectFile.Root
                    .Elements("ItemGroup")
                    .Elements("PackageReference")
                    .Where(x => x.HasAttributes && x.Attributes().Any(a => a.Name == "Include" && a.Value == dependency.PackageId))
                    .Attributes().Where(a => a.Name == "Version").First().Value = dependency.Version;
                }

                // Get the XML file.
                var xml = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    projectFile.ToString();

                // Build the update URL.
                var updateUrl = string.Format(
                    BitbucketUpdateUrlTemplate,
                    repo.TeamName,
                    repo.RepoSlug);
                var updateRequest = new HttpRequestMessage(HttpMethod.Post, updateUrl);
                updateRequest.Headers.Add("Authorization", $"Basic {auth}");

                // Build the form data.
                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>(repo.ProjectFile, xml));
                postData.Add(new KeyValuePair<string, string>("message", "Dependency update with squid."));
                postData.Add(new KeyValuePair<string, string>("branch", SquidConfig.BranchName));
                var postContent = new FormUrlEncodedContent(postData);
                updateRequest.Content = postContent;

                // Update the file.
                var updateResponse = await _client.SendAsync(updateRequest);

                if (updateResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Updating the project file '{repo.ProjectFile}' of '{repo.RepoSlug}' succeeded.");

                    // Build the PR URL.
                    var prUrl = string.Format(
                        BitbucketPRUrlTemplate,
                        repo.TeamName,
                        repo.RepoSlug);

                    // Build the PR model.
                    var prModel = new Q.Squid.Models.Bitbucket.PullRequest()
                    {
                        title = SquidConfig.BranchName,
                        description = "Dependency update with squid.",
                        close_source_branch = true,
                        source = new Q.Squid.Models.Bitbucket.Target()
                        {
                            branch = new Q.Squid.Models.Bitbucket.Branch()
                            {
                                name = SquidConfig.BranchName
                            }
                        },
                        destination = new Q.Squid.Models.Bitbucket.Target()
                        {
                            branch = new Q.Squid.Models.Bitbucket.Branch()
                            {
                                name = repo.Branch
                            }
                        }
                    };

                    // Set the reviewer.
                    if (!string.IsNullOrWhiteSpace(reviewer) && reviewer != repo.Username)
                    {
                        prModel.reviewers = new List<Models.Bitbucket.Author>
                        {
                            new Models.Bitbucket.Author()
                            {
                                username = reviewer
                            }
                        };
                    }

                    // Prepare PR request.
                    var prRequest = new HttpRequestMessage(HttpMethod.Post, prUrl);
                    prRequest.Headers.Add("Authorization", $"Basic {auth}");
                    var prModelString = JsonConvert.SerializeObject(prModel, SquidConfig.SerializerSetting);
                    var prContent = new StringContent(prModelString, Encoding.UTF8, "application/json");
                    prRequest.Content = prContent;

                    // Create the PR.
                    var prResponse = await _client.SendAsync(prRequest);

                    if (prResponse.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Creating the PR in '{repo.RepoSlug}' succeeded.");
                    }
                    else
                    {
                        var failure = await prResponse.Content.ReadAsStringAsync();
                        _logger.LogError($"Creating the PR in '{repo.RepoSlug}' failed. Error: {failure}.");
                    }
                }
                else
                {
                    var failure = await updateResponse.Content.ReadAsStringAsync();
                    _logger.LogError($"Updating the project file '{repo.ProjectFile}' of '{repo.RepoSlug}' failed. Error: {failure}.");
                }
            }
        }

        public static string GetName()
        {
            return "DependencyUpdateJob";
        }
    }
}