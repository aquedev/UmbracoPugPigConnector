using Umbraco.Cms.Web;
using Umbraco.Pugpig.Core.Models;

namespace Umbraco.Pugpig.Core.Interfaces
{
    public interface IPugpigRepository
    {
        Feed CreateEditionList(string publicationName, UmbracoHelper umbracoHelper);
    }
}
