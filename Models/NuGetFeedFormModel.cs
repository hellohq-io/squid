using System;
using System.Collections.Generic;

namespace Q.Squid.Models
{
    public class NuGetFeedFormModel
    {
        public string Name { get; set; }
        public string FeedURL { get; set; }
        public string ApiKeyHeaderName { get; set; }
        public string ApiKey { get; set; }
    }
}