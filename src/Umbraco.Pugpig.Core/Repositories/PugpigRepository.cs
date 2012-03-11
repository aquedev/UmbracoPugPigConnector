using System;
using System.Collections.Generic;
using Umbraco.Cms.Web;
using Umbraco.Cms.Web.Context;
using System.Linq;
using Umbraco.Framework.Diagnostics;
using Umbraco.Pugpig.Core.Controllers;
using Umbraco.Pugpig.Core.Interfaces;
using Umbraco.Pugpig.Core.Models;

namespace Umbraco.Pugpig.Core.Repositories
{
    public static class ContentFields
    {
        public const string PAGE_URL = "contentUrl";
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
            var parentName = m_context.Application.Hive.QueryContent()
                .Where(x => x.ContentType.Alias == "iBookEdition").First().ParentContent().Name;

            var editions = m_context.Application.Hive.QueryContent()
                .Where(x => x.ContentType.Alias == "iBookEdition")
                .ToList();
            editions = editions.Where(x => x.ParentContent().Name == publicationName).ToList();

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
                                         Title = edition.Name,
                                         Updated = DateTime.Now,
                                         Image = new Image() {Url = imageUrl}

                });
            }

            return feed;
        }

        public Book CreateBookList(string bookName, string publicationName, UmbracoHelper umbracoHelper)
        {
            var pages = m_context.Application.Hive.QueryContent()
                .Where(x => x.ContentType.Alias == "iBookPage")
                
                .ToList();

            pages = pages.Where(x => x.ParentContent().Name == bookName).ToList();
            LogHelper.TraceIfEnabled<PugpigRepository>("We found {0} editions for {1}.", () => pages.Count, () => bookName);

            Book book = new Book();
            book.LastUpdated = DateTime.Now;
            book.Pages = new List<Page>();
            book.Title = bookName;
            foreach (var content in pages)
            {
                Page page = new Page();
                page.PageUrl = content.DynamicField(ContentFields.PAGE_URL);
                book.Pages.Add(page);
            }
            return book;
        }

        public List<PublicationSumaryModel> GetAllPublications(UmbracoHelper umbracoHelper)
        {
            List<PublicationSumaryModel> publicationSumaryModels = new List<PublicationSumaryModel>();

            var editions = m_context.Application.Hive.QueryContent()
               .Where(x => x.ContentType.Alias == "iBookFeed")
               .ToList();

            foreach (var edition in editions)
            {
                var imageUrl = umbracoHelper.GetMediaUrl(edition.Id, "coverImage");
                var model = new PublicationSumaryModel
                                {
                                    FeedUrl =
                                        String.Format("umbraco/pugpig/PugpigSurface/Editions?publicationName={0}",
                                                      edition.Name),
                                    Name = edition.Name,
                                    ImageUrl = imageUrl
                                };
                publicationSumaryModels.Add(model);

            }
            return publicationSumaryModels;
        }
    }
}
