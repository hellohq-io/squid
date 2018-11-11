namespace Q.Squid.Models
{
    public class PackageDependency
    {
        /// <summary>
        /// The id of the package.
        /// </summary>
        /// <returns></returns>
        public string PackageId { get; set; }

        /// <summary>
        /// The current version of the dependency.
        /// </summary>
        /// <returns></returns>
        public string Version { get; set; }

        /// <summary>
        /// The latest version of the dependency.
        /// </summary>
        /// <returns></returns>
        public string LatestVersion { get; set; }

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
}