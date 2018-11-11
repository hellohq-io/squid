using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Examples;

namespace Q.Squid.Models.Examples
{
    public class DependencyExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<DependenciesResponse>()
            {
                new DependenciesResponse()
                {
                    Repository = new RepositoryConfigExample().GetExamples() as RepositoryConfigModel,
                    PackageDependencies = new List<PackageDependency>()
                    {
                        new PackageDependency()
                        {
                            PackageId = "squid",
                            Version = "0.9",
                            LatestVersion = "1.0"
                        }
                    }
                },
                new DependenciesResponse()
                {
                    Repository = new RepositoryConfigExample().GetExamples() as RepositoryConfigModel,
                    PackageDependencies = new List<PackageDependency>()
                    {
                        new PackageDependency()
                        {
                            PackageId = "squid-ui",
                            Version = "0.4",
                            LatestVersion = "0.5"
                        }
                    }
                }
            };
        }
    }

    public class DependencyUpdateExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new DependencyUpdate()
            {
                RepositoryId = "452af5",
                Updates = new List<Update>()
                {
                    new Update()
                    {
                        PackageId = "squid",
                        Version = "1.0.0"
                    }
                }
            };
        }
    }
}