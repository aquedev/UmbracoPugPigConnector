using Umbraco.Pugpig.Core.Interfaces;

namespace Umbraco.Pugpig.Core.Settings
{
    public class BookSettings : IBookSettings
    {
        public string BookName { get; set; }

        public string BaseUrl { get; set; }

        public string AuthourName { get; set; }
    }
}