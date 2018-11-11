using System;
using System.Collections.Generic;

namespace Q.Squid.Models
{
    public class Update
    {
        /// <summary>
        /// The id of the package dependency to update.
        /// </summary>
        /// <returns></returns>
        public string PackageId { get; set; }

        /// <summary>
        /// The version to update to.
        /// </summary>
        /// <returns></returns>
        public string Version { get; set; }
    }
}