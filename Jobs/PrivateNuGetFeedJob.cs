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
    public class PrivateNuGetFeedJob : IJob
    {
        private readonly ILogger _logger;
        private readonly HttpClient _client;
        private readonly NugetFeedConfigRepository _repo;

        public PrivateNuGetFeedJob(
            ILogger<PrivateNuGetFeedJob> logger,
            HttpClient client,
            NugetFeedConfigRepository repo)
        {
            _logger = logger;
            _client = client;
            _repo = repo;
        }

        public async Task Execute()
        {
            _logger.LogTrace($"Starting PrivateNuGetFeedJob.");
            var feeds = await _repo.GetAll();

            foreach (var feed in feeds)
            {
                var packages = await GetPackages(feed);

                if (packages != null)
                {
                    foreach (var package in packages)
                    {
                        SquidStore.Packages[package.id] = package;
                    }
                }
            }
        }

        private async Task<List<PackageDetails>> GetPackages(NugetFeedConfig feed)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, feed.FeedURL);
            var apiKey = Crypto.DecryptString(feed.ApiKey, SquidConfig.CryptoKey);
            if (string.IsNullOrWhiteSpace(feed.ApiKeyHeaderName))
                request.Headers.Add("X-NuGet-ApiKey", apiKey);
            else
                request.Headers.Add(feed.ApiKeyHeaderName, apiKey);

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var packageVersions = JsonConvert.DeserializeObject<PackageVersions>(content);

                if (!feed.Valid)
                {
                    feed.ErrorMessage = null;
                    feed.Valid = true;

                    await _repo.Update(feed);
                }

                if (packageVersions != null)
                {
                    var packageDetails = packageVersions.data;

                    foreach (var package in packageDetails)
                    {
                        package.FeedId = feed.Id;
                        package.FeedName = feed.Name;
                    }

                    return packageDetails;
                }
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                feed.ErrorMessage = content;
                feed.Valid = false;

                await _repo.Update(feed);
            }

            return null;
        }

        public static string GetName()
        {
            return "PrivateNuGetFeedJob";
        }
    }
}