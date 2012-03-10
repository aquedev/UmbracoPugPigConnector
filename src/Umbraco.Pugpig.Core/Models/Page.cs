using System;

namespace Umbraco.Pugpig.Core.Models
{
    public class Page
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Updated { get; set; }
        public string PageUrl { get; set; }
    }
}