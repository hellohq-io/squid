using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Q.Squid.Models
{
    public class DependencyUpdate
    {
        /// <summary>
        /// The id of the repository to update.
        /// </summary>
        /// <returns></returns>
        public string RepositoryId { get; set; }

        /// <summary>
        /// The list of dependencies and new versions to update to.
        /// </summary>
        /// <returns></returns>
        public List<Update> Updates { get; set; }
    }
}