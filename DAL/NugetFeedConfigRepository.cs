using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Q.Squid.DAL
{
    public class NugetFeedConfigRepository : AbstractRepository<NugetFeedConfig>
    {
        public NugetFeedConfigRepository(string connectionString, string databaseName)
            : base(connectionString, databaseName, "nugetfeedconfig")
        {
        }
    }
}