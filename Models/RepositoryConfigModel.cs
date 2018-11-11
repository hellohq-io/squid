using System;
using System.Collections.Generic;

namespace Q.Squid.Models
{
    public class RepositoryConfigModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RepoSlug { get; set; }
        public string TeamName { get; set; }
        public string Branch { get; set; }
        public string ProjectFile { get; set; }
        public bool Valid { get; set; }
        public string ErrorMessage { get; set; }
    }
}