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
using Q.Squid.Models.Bitbucket;
using Q.Squid.Store;
using Q.Squid.Utils;

namespace Q.Squid.Jobs
{
    public class CheckPullRequestStatusJob : IJob
    {
        private const string BitbucketPRUrlTemplate = "https://api.bitbucket.org/2.0/repositories/{0}/{1}/pullrequests/";
        private const string BitbucketPRStatusUrlTemplate = "https://api.bitbucket.org/2.0/repositories/{0}/{1}/pullrequests/{2}/statuses";
        private const string BitbucketPRMergeUrlTemplate = "https://api.bitbucket.org/2.0/repositories/{0}/{1}/pullrequests/{2}/merge";

        private readonly ILogger _logger;
        private readonly RepositoryConfigRepository _repo;
        private readonly HttpClient _client;

        public CheckPullRequestStatusJob(
            ILogger<CheckPullRequestStatusJob> logger,
            RepositoryConfigRepository repo,
            HttpClient client)
        {
            _logger = logger;
            _repo = repo;
            _client = client;
        }

        public async Task Execute()
        {
            _logger.LogTrace($"Starting CheckPullRequestStatusJob.");
            var repos = await _repo.GetAll();

            foreach (var repo in repos)
            {
                var prs = await GetPullRequests(repo);

                if (prs != null)
                {
                    var squidPrs = prs.Where(p => p.title == SquidConfig.BranchName);
                    foreach (var squidPr in squidPrs)
                    {
                        // Get the status of the PR.
                        var prStatus = await GetPRStatus(repo, squidPr.id);

                        if (prStatus != null && prStatus.state == "SUCCESSFUL")
                        {
                            // The squid PR built successfully, let's merge it.
                            await MergePR(repo, squidPr.id);
                        }
                        else
                        {
                            string status = prStatus != null ? prStatus.state : "unknown";
                            _logger.LogInformation($"Skipped merging pull request {squidPr.id} of '{repo.RepoSlug}' because it is '{status}'.");
                        }
                    }
                }
            }
        }

        private async Task<List<PullRequest>> GetPullRequests(RepositoryConfig repo)
        {
            var url = string.Format(
                BitbucketPRUrlTemplate,
                repo.TeamName,
                repo.RepoSlug);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var password = Crypto.DecryptString(repo.Password, SquidConfig.CryptoKey);
            var auth = Base64.ToBase64(Encoding.UTF8, $"{repo.Username}:{password}");
            request.Headers.Add("Authorization", $"Basic {auth}");

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var prResponse = JsonConvert.DeserializeObject<PullRequestsResponse>(content);

                if (prResponse != null && prResponse.values != null)
                {
                    _logger.LogInformation($"Retrieved the pull requests of '{repo.RepoSlug}'.");
                    return prResponse.values;
                }
            }

            var failure = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Getting the pull requests of '{repo.RepoSlug}' failed. Error: {failure}.");

            return null;
        }

        private async Task<PullRequestStatus> GetPRStatus(RepositoryConfig repo, int prId)
        {
            var url = string.Format(
                BitbucketPRStatusUrlTemplate,
                repo.TeamName,
                repo.RepoSlug,
                prId);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var password = Crypto.DecryptString(repo.Password, SquidConfig.CryptoKey);
            var auth = Base64.ToBase64(Encoding.UTF8, $"{repo.Username}:{password}");
            request.Headers.Add("Authorization", $"Basic {auth}");

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var prResponse = JsonConvert.DeserializeObject<PullRequestStatusesResponse>(content);

                if (prResponse != null && prResponse.values != null && prResponse.values.Any())
                {
                    _logger.LogInformation($"Retrieved the pull request status for PR {prId} of '{repo.RepoSlug}'.");
                    return prResponse.values.First();
                }
            }

            var failure = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Getting the pull request status for PR {prId} of '{repo.RepoSlug}' failed. Error: {failure}.");

            return null;
        }

        private async Task MergePR(RepositoryConfig repo, int prId)
        {
            var url = string.Format(
                BitbucketPRMergeUrlTemplate,
                repo.TeamName,
                repo.RepoSlug,
                prId);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var password = Crypto.DecryptString(repo.Password, SquidConfig.CryptoKey);
            var auth = Base64.ToBase64(Encoding.UTF8, $"{repo.Username}:{password}");
            request.Headers.Add("Authorization", $"Basic {auth}");

            var prModel = new Q.Squid.Models.Bitbucket.MergePullRequestModel()
            {
                type = "pullrequest",
                message = "Dependency update with squid",
                close_source_branch = false,
                merge_strategy = "merge_commit"
            };

            var prModelString = JsonConvert.SerializeObject(prModel, SquidConfig.SerializerSetting);
            var prContent = new StringContent(prModelString, Encoding.UTF8, "application/json");
            request.Content = prContent;

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Merging the pull request for PR {prId} of '{repo.RepoSlug}' successful.");
            }
            else
            {
                var failure = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Merging the pull request for PR {prId} of '{repo.RepoSlug}' failed. Error: {failure}.");
            }
        }

        public static string GetName()
        {
            return "CheckPullRequestStatusJob";
        }
    }
}