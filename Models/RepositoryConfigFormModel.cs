using System;
using System.Collections.Generic;

namespace Q.Squid.Models
{
    public class RepositoryConfigFormModel
    {
        public string Name { get; set; }
        public string RepoSlug { get; set; }
        public string TeamName { get; set; }
        public string Branch { get; set; }
        public string ProjectFile { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}