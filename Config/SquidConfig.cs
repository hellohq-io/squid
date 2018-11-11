using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Q.Squid.Config
{
    public static class SquidConfig
    {
        public static readonly JsonSerializerSettings SerializerSetting =
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

        /// <summary>
        /// The branch name to use for the dependency update.
        /// </summary>
        /// <returns></returns>
        public static string BranchName { get; set; }

        /// <summary>
        /// The key used to encrypt and decrypt secrets.
        /// </summary>
        /// <returns></returns>
        public static string CryptoKey { get; set; }
        public static int DependencyJobInterval { get; set; }
        public static int PullRequestJobInterval { get; set; }
        public static int NuGetJobInterval { get; set; }
        public static int PrivateFeedJobInterval { get; set; }
    }
}