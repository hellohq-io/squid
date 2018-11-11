using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.Extensions.Logging;

using MongoDB.Bson;

using Newtonsoft.Json;

using Q.Squid.DAL;
using Q.Squid.Models;
using Q.Squid.Store;
using Q.Squid.Utils;

namespace Q.Squid.Jobs
{
    public class NuGetFeedJob : IJob
    {
        private const string NuGetPackageFeedUrl = "https://api.nuget.org/v3/registration3/{0}/index.json";

        private readonly ILogger _logger;
        private readonly HttpClient _client;

        public NuGetFeedJob(
            ILogger<NuGetFeedJob> logger,
            HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task Execute()
        {
            _logger.LogTrace($"Starting NuGetFeedJob.");
            var packages = SquidStore.Dependencies.Values
                .SelectMany(d => d.PackageDependencies.Select(s => s.PackageId))
                .Distinct().ToList();

            foreach (var feed in packages)
            {
                var package = await GetPackageDetails(feed);
                if (package != null)
                {
                    if (SquidStore.Packages.ContainsKey(feed))
                    {
                        // Replace only if the existing package is also from the public feed.
                        // This way, private feed packages get higher priority.
                        var existingPackage = SquidStore.Packages[feed];
                        if (existingPackage.FeedId == null)
                        {
                            SquidStore.Packages[feed] = package;
                        }
                    }
                    else
                    {
                        SquidStore.Packages[feed] = package;
                    }
                }
            }
        }

        private async Task<PackageDetails> GetPackageDetails(string packageId)
        {
            var url = string.Format(NuGetPackageFeedUrl, packageId.ToLower());
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var package = JsonConvert.DeserializeObject<NuGetFeedResponse>(content);

                if (package != null && package.items != null &&
                    package.items.Any() && package.items.First().items != null &&
                    package.items.First().items.Any())
                {
                    var details = package.items.First().items.Where(c => c.catalogEntry != null).Last().catalogEntry;

                    if (details != null)
                    {
                        return new PackageDetails()
                        {
                            id = packageId,
                            version = details.version,
                            FeedName = "NuGet" // From the public NuGet feed.
                        };
                    }
                }
            }
            return null;
        }

        public static string GetName()
        {
            return "NuGetFeedJob";
        }
    }
}