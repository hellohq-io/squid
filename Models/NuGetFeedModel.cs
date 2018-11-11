using System;
using System.Collections.Generic;

namespace Q.Squid.Models
{
    public class NuGetFeedModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FeedURL { get; set; }
        public bool Valid { get; set; }
        public string ErrorMessage { get; set; }
    }
}