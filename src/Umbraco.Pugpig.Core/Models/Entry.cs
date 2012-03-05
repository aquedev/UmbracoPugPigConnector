using System;

namespace Umbraco.Pugpig.Core.Models
{
    public class Entry
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Updated { get; set; }
        public string AuthourName { get; set; }
        public string Summary { get; set; }
        public Image Image { get; set; }
    }
}
