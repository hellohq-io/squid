using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Examples;

namespace Q.Squid.Models.Examples
{
    public class RepositoryConfigExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<RepositoryConfigModel>()
            {
                new RepositoryConfigModel(){
                    Id = "5xsyut",
                    Name = "Tasks Service",
                    TeamName = "sQuid",
                    Branch = "next",
                    RepoSlug = "tasks-service",
                    ProjectFile = "services/tasks.csproj",
                },
                new RepositoryConfigModel(){
                    Id = "t6syvt",
                    Name = "Projects Service",
                    TeamName = "sQuid",
                    Branch = "next",
                    RepoSlug = "projects-service",
                    ProjectFile = "services/projects.csproj",
                }
            };
        }
    }

    public class RepositoryConfigExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new RepositoryConfigModel()
            {
                Id = "5xsyut",
                Name = "Tasks Service",
                TeamName = "sQuid",
                Branch = "next",
                RepoSlug = "tasks-service",
                ProjectFile = "services/tasks.csproj",
            };
        }
    }

    public class RepositoryConfigFormExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new RepositoryConfigFormModel()
            {
                Name = "Tasks Service",
                TeamName = "sQuid",
                Branch = "next",
                RepoSlug = "tasks-service",
                ProjectFile = "services/tasks.csproj",
                Username = "squid",
                Password = "squidslovewater"
            };
        }
    }
}