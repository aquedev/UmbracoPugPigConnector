using System.Collections.Generic;
using Umbraco.Cms.Web;
using Umbraco.Pugpig.Core.Controllers;
using Umbraco.Pugpig.Core.Models;

namespace Umbraco.Pugpig.Core.Interfaces
{
    public interface IPugpigRepository
    {
        Feed CreateEditionList(string publicationName, UmbracoHelper umbracoHelper);
        Book CreateBookList(string bookName, string publicationName, UmbracoHelper umbracoHelper);
        List<PublicationSumaryModel> GetAllPublications(UmbracoHelper umbracoHelper);
    }
}
