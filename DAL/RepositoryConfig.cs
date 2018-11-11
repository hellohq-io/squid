using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Q.Squid.DAL
{
    [BsonIgnoreExtraElements]
    public class RepositoryConfig : SquidBaseModel
    {
        public string Name { get; set; }
        public string RepoSlug { get; set; }
        public string TeamName { get; set; }
        public string Branch { get; set; }
        public string ProjectFile { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Valid { get; set; }
        public string ErrorMessage { get; set; }
    }
}