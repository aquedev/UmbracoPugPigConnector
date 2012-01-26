using System;

namespace Umbraco.Pugpig.Core.Models
{
    public class Entry
    {
        public Image Image { get; set; }
           
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Updated { get; set; }
        public string AuthourName { get; set; }
        public string Summary { get; set; }
    }

     public class Image
     {
         public string Url { get; set; }
     }
}
