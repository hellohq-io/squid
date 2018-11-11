using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MongoDB.Bson;
using Q.Squid.Models;

namespace Q.Squid.Store
{
    public static class SquidStore
    {
        /// <summary>
        /// The dependencies of the repository by id.
        /// </summary>
        public static ConcurrentDictionary<string, Dependencies> Dependencies;

        /// <summary>
        /// The package details of the repository by id.
        /// </summary>
        public static ConcurrentDictionary<string, PackageDetails> Packages;
    }
}