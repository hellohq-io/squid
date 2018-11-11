using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using AutoMapper;

using Microsoft.Extensions.Logging;

using MongoDB.Bson;

using Q.Squid.Config;
using Q.Squid.DAL;
using Q.Squid.Models;
using Q.Squid.Store;
using Q.Squid.Utils;

namespace Q.Squid.Jobs
{
    public class DependencyJob : IJob
    {
        private const string BitbucketUrlTemplate = "https://api.bitbucket.org/2.0/repositories/{0}/{1}/src/{2}/{3}";

        private readonly ILogger _logger;
        private readonly RepositoryConfigRepository _repo;
        private readonly HttpClient _client;
        private readonly IMapper _mapper;

        public DependencyJob(
            ILogger<DependencyJob> logger,
            RepositoryConfigRepository repo,
            HttpClient client,
            IMapper mapper)
        {
            _logger = logger;
            _repo = repo;
            _client = client;
            _mapper = mapper;
        }

        public async Task Execute()
        {
            _logger.LogTrace($"Starting DependencyJob.");
            var repos = await _repo.GetAll();

            foreach (var repo in repos)
            {
                var repoModel = _mapper.Map<RepositoryConfigModel>(repo);
                var dependencies = await GetDependencies(repo);

                if (dependencies != null)
                {
                    Dependencies serviceDependencies;
                    if (SquidStore.Dependencies.ContainsKey(repo.Id))
                    {
                        serviceDependencies = SquidStore.Dependencies[repo.Id];
                        serviceDependencies.Repository = repoModel;
                        serviceDependencies.PackageDependencies = dependencies;
                        SquidStore.Dependencies[repo.Id] = serviceDependencies;
                    }
                    else
                    {
                        serviceDependencies = new Dependencies()
                        {
                            Repository = repoModel,
                            PackageDependencies = dependencies
                        };

                        SquidStore.Dependencies.TryAdd(repo.Id, serviceDependencies);
                    }
                }
            }
        }

        private async Task<List<PackageDependency>> GetDependencies(RepositoryConfig repo)
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

                XDocument projectFile = XDocument.Parse(content);
                List<PackageDependency> dependencies = projectFile.Root
                    .Elements("ItemGroup")
                    .Elements("PackageReference")
                    .Select(x => new PackageDependency
                    {
                        PackageId = (string)x.Attribute("Include"),
                        Version = (string)x.Attribute("Version")
                    })
                    .Where(x => !string.IsNullOrWhiteSpace(x.PackageId) && !string.IsNullOrWhiteSpace(x.Version))
                    .ToList<PackageDependency>();

                if (!repo.Valid)
                {
                    repo.ErrorMessage = null;
                    repo.Valid = true;

                    await _repo.Update(repo);
                }

                return dependencies;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                repo.ErrorMessage = content;
                repo.Valid = false;

                await _repo.Update(repo);

                return null;
            }
        }

        public static string GetName()
        {
            return "DependencyJob";
        }
    }
}