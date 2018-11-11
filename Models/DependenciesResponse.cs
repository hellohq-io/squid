using System;
using System.Collections.Generic;
using Q.Squid.Store;

namespace Q.Squid.Models
{
    public class DependenciesResponse : Dependencies
    {
        public List<Update> Updates { get; set; }
    }
}