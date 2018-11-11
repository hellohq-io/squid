using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Examples;

namespace Q.Squid.Models.Examples
{
    public class NuGetFeedExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<NuGetFeedModel>()
            {
                new NuGetFeedModel(){
                    Id = "a76xsyut",
                    Name = "squid MyGet",
                    FeedURL = "https://www.myget.org/F/squid/",
                },
                new NuGetFeedModel(){
                    Id = "a46xsyzt",
                    Name = "squid NuGet",
                    FeedURL = "https://www.nuget.org/F/squid/",
                }
            };
        }
    }

    public class NuGetFeedExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NuGetFeedModel()
            {
                Id = "a76xsyut",
                Name = "squid MyGet",
                FeedURL = "https://www.myget.org/F/squid/",
            };
        }
    }

    public class NuGetFeedFormExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NuGetFeedFormModel()
            {
                Name = "squid MyGet",
                FeedURL = "https://www.myget.org/F/squid/",
                ApiKey = "secret-myget-key",
                ApiKeyHeaderName = "X-NuGet-ApiKey"
            };
        }
    }
}