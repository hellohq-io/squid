using System;
using System.Collections.Generic;

namespace Q.Squid.Models
{
    public class Dependency
    {
        public string id { get; set; }
        public string range { get; set; }
        public string registration { get; set; }
    }

    public class DependencyGroup
    {
        public List<Dependency> dependencies { get; set; }
    }

    public class CatalogEntry
    {
        public string authors { get; set; }
        public List<DependencyGroup> dependencyGroups { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string id { get; set; }
        public string language { get; set; }
        public string licenseUrl { get; set; }
        public bool listed { get; set; }
        public string minClientVersion { get; set; }
        public string packageContent { get; set; }
        public string projectUrl { get; set; }
        public DateTime published { get; set; }
        public bool requireLicenseAcceptance { get; set; }
        public string summary { get; set; }
        public List<string> tags { get; set; }
        public string title { get; set; }
        public string version { get; set; }
    }

    public class Item
    {
        public string commitId { get; set; }
        public DateTime commitTimeStamp { get; set; }
        public int count { get; set; }
        public List<Item> items { get; set; }
        public string parent { get; set; }
        public string lower { get; set; }
        public string upper { get; set; }
        public CatalogEntry catalogEntry { get; set; }
    }

    public class NuGetFeedResponse
    {
        public string commitId { get; set; }
        public DateTime commitTimeStamp { get; set; }
        public int count { get; set; }
        public List<Item> items { get; set; }
    }
}