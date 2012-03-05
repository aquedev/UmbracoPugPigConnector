using Umbraco.Pugpig.Core.Interfaces;

namespace Umbraco.Pugpig.Core.Settings
{
    public class FeedSettings : IFeedSettings
    {
        public string PublicationName { get; set; }
        public string BaseUrl { get; set; }
    }
}
