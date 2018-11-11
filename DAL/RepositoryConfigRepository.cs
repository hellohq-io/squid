using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Q.Squid.DAL
{
    public class RepositoryConfigRepository : AbstractRepository<RepositoryConfig>
    {
        public RepositoryConfigRepository(string connectionString, string databaseName)
            : base(connectionString, databaseName, "repositoryconfig")
        {
        }
    }
}