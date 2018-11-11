using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Hangfire;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Q.Squid.DAL;
using Q.Squid.Jobs;
using Q.Squid.Models;
using Q.Squid.Models.Examples;
using Q.Squid.Store;

using Swashbuckle.AspNetCore.Examples;

namespace Q.Squid.Controllers
{
    [Route("api/[controller]")]
    public class DependenciesController : Controller
    {
        private readonly RepositoryConfigRepository _repo;
        private readonly ILogger<DependencyUpdateJob> _logger;
        private readonly HttpClient _client;

        public DependenciesController(
            RepositoryConfigRepository repo,
            ILogger<DependencyUpdateJob> logger,
            HttpClient client)
        {
            _repo = repo;
            _logger = logger;
            _client = client;
        }

        /// <summary>
        /// Returns the dependencies and latest versions of all repositories.
        /// </summary>
        /// <remarks>
        /// Returns the dependencies and latest versions of all repositories.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<DependenciesResponse>), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(DependencyExamples))]
        public IEnumerable<DependenciesResponse> Get()
        {
            List<DependenciesResponse> response = new List<DependenciesResponse>();

            var dependencies = SquidStore.Dependencies.Values;
            foreach (var dep in dependencies)
            {
                DependenciesResponse depResponse = new DependenciesResponse();
                depResponse.Repository = dep.Repository;
                depResponse.PackageDependencies = dep.PackageDependencies;
                depResponse.Updates = new List<Update>();

                foreach (var dependency in depResponse.PackageDependencies)
                {
                    if (SquidStore.Packages.ContainsKey(dependency.PackageId))
                    {
                        var package = SquidStore.Packages[dependency.PackageId];
                        dependency.LatestVersion = package.version;
                        dependency.FeedId = package.FeedId;
                        dependency.FeedName = package.FeedName;

                        var cleanCurrentVersion = dependency.Version.Split('-').First();
                        var cleanLatestVersion = dependency.Version.Split('-').First();

                        var currentVersion = new System.Version(cleanCurrentVersion);
                        var latestVersion = new System.Version(cleanLatestVersion);

                        if (currentVersion.CompareTo(latestVersion) > 0)
                        {
                            depResponse.Updates.Add(new Update()
                            {
                                PackageId = package.id,
                                Version = package.version
                            });
                        }
                    }
                }

                response.Add(depResponse);
            }

            return response;
        }

        /// <summary>
        /// Updates the dependencies of the specified repository.
        /// </summary>
        /// <param name="model"></param>
        /// <remarks>
        /// Updates the dependencies of the specified repository.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [SwaggerRequestExample(typeof(DependencyUpdate), typeof(DependencyUpdateExample))]
        public async Task<IActionResult> UpdateDependencies([FromBody] DependencyUpdate model)
        {
            var repo = await _repo.Get(model.RepositoryId);
            if (repo == null)
                return NotFound();

            string reviewer = string.Empty;
            if (Request.Cookies.ContainsKey("squid-bb-user"))
            {
                string squidBbUserName = Request.Cookies["squid-bb-user"];
                if (!string.IsNullOrWhiteSpace(squidBbUserName))
                {
                    reviewer = Encoding.UTF8.GetString(Convert.FromBase64String(squidBbUserName));
                }
            }

            BackgroundJob.Enqueue(
                () => new DependencyUpdateJob(
                        _logger,
                        _client)
                        .Execute(repo, model.Updates, reviewer));

            return Accepted();
        }
    }
}
