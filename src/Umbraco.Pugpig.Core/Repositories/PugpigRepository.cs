using System;
using System.Collections.Generic;
using Umbraco.Cms.Web;
using Umbraco.Cms.Web.Context;
using System.Linq;
using Umbraco.Cms.Web.Model;
using Umbraco.Framework;
using Umbraco.Framework.Diagnostics;
using Umbraco.Pugpig.Core.Interfaces;
using Umbraco.Pugpig.Core.Models;

namespace Umbraco.Pugpig.Core.Repositories
{
    public static class ContentFields
    {
        public const string COVER_IMAGE = "coverImage";
        public const string SUMMARY = "summary";
        public const string AUTHOUR_NAME = "authorName";
        public const string TITLE = "title";
    }

    public class PugpigRepository : IPugpigRepository
    {
        private readonly IRoutableRequestContext m_context;

        public PugpigRepository(IRoutableRequestContext context)
        {
            m_context = context;
        }

        public Feed CreateEditionList(string publicationName, UmbracoHelper umbracoHelper)
        {
            var editions = m_context.Application.Hive.QueryContent()
                .Where(x => x.ContentType.Alias == "iBookEdition")
               // .Where(x => x.ParentContent(). == publicationName)
                .ToList();
             LogHelper.TraceIfEnabled<PugpigRepository>("We found {0} editions for {1}.",() => editions.Count, () => publicationName);       

            Feed feed = new Feed();
            feed.Entries = new List<Entry>();
             
            foreach (var edition in editions)
            {
                var imageUrl = umbracoHelper.GetMediaUrl(edition.Id, "coverImage");
                feed.Entries.Add(new Entry()
                                     {
                                         AuthourName = edition.DynamicField(ContentFields.AUTHOUR_NAME),
                                         Id = edition.Id.ToFriendlyString(),
                                         Summary = edition.DynamicField(ContentFields.SUMMARY),
                                         Title = edition.DynamicField(ContentFields.TITLE),
                                         Updated = DateTime.Now,
                                         Image = new Image() {Url = imageUrl}

                });
            }

            return feed;
        }
    }
}
