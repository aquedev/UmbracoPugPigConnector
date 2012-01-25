using System;
using System.Collections.Generic;

namespace Umbraco.Pugpig.Core.Models
{
    public class Feed
    {
        public DateTime LastUpdated { get; set; }
        public List<Entry> Entries { get; set; }
        public string Title { get; set; }
    }
}
