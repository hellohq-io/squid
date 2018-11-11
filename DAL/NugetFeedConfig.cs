using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Q.Squid.DAL
{
    [BsonIgnoreExtraElements]
    public class NugetFeedConfig : SquidBaseModel
    {
        public string Name { get; set; }
        public string FeedURL { get; set; }
        public string ApiKeyHeaderName { get; set; }
        public string ApiKey { get; set; }
        public bool Valid { get; set; }
        public string ErrorMessage { get; set; }
    }
}