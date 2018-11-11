using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Q.Squid.DAL;

namespace Q.Squid.Models
{
    public class Dependencies
    {
        public RepositoryConfigModel Repository;
        public List<PackageDependency> PackageDependencies;
    }
}