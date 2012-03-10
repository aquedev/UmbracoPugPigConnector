using System;
using System.Collections.Generic;

namespace Umbraco.Pugpig.Core.Models
{
    public class Book
    {
        public DateTime LastUpdated { get; set; }
        public List<Page> Pages { get; set; }
        public string Title { get; set; }
    }
}