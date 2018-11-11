using System;
using System.Collections.Generic;

namespace Q.Squid.Models
{
    public class Version
    {
        public string version { get; set; }
        public int downloads { get; set; }
    }

    public class PackageDetails
    {
        public string registration { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string summary { get; set; }
        public string title { get; set; }
        public string iconUrl { get; set; }
        public object licenseUrl { get; set; }
        public object projectUrl { get; set; }
        public List<object> tags { get; set; }
        public List<string> authors { get; set; }
        public int totaldownloads { get; set; }
        public bool verified { get; set; }
        public string version { get; set; }
        public List<Version> versions { get; set; }

        // Custom Properties.

        /// <summary>
        /// The name of the feed.
        /// </summary>
        /// <returns></returns>
        public string FeedName { get; set; }

        /// <summary>
        /// The id of the feed.
        /// Empty if this is from the public NuGet feed.
        /// </summary>
        /// <returns></returns>
        public string FeedId { get; set; }
    }

    public class PackageVersions
    {
        public int totalHits { get; set; }
        public string index { get; set; }
        public DateTime lastReopen { get; set; }
        public List<PackageDetails> data { get; set; }
    }
}