using System;

namespace Umbraco.Pugpig.Core.Models
{
    public class Entry
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Updated { get; set; }
        public string AuthourName { get; set; }
        public string Summary { get; set; }
    }
}
