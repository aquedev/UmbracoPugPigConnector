using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Umbraco.Pugpig.Core.Interfaces;
using Umbraco.Pugpig.Core.Models;

namespace Umbraco.Pugpig.Core
{
    public class XmlFormatter : IXmlFormatter
    {
        private readonly IFeedSettings m_feedInfo;

        public XmlFormatter(IFeedSettings feedInfo)
        {
            m_feedInfo = feedInfo;
        }

        public XmlDocument GenerateXml(Feed feed)
        {
            var element = new XElement("feed",
                                       new XElement("id", m_feedInfo.FeedId),
                                       GetLinkElement(),
                                       new XElement("title", feed.Title),
                                       new XElement("updated", feed.LastUpdated.ToString("yyyy-MM-ddTH:mm:sszzz")),
                                       GetEntries(feed.Entries));

            XmlReader xmlReader = element.CreateReader();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlReader);

            SetNamespaces(xmlDoc);

            return xmlDoc;
        }

        private XElement GetLinkElement()
        {
            XElement link = new XElement("link");
            link.SetAttributeValue("rel", "self");
            link.SetAttributeValue("type", "application/atom+xml;profile=opds-catalog;kind=acquisition");
            link.SetAttributeValue("href", String.Concat("http://", m_feedInfo.BaseUrl, "/pugpig/editions.xml"));
            return link;
        }

        private void SetNamespaces(XmlDocument xmlDoc)
        {
            if (xmlDoc.DocumentElement != null)
            {
                xmlDoc.DocumentElement.SetAttribute("xmlns", "http://www.w3.org/2005/Atom");
                xmlDoc.DocumentElement.SetAttribute("xmlns:dcterms", "http://purl.org/dc/terms/");
                xmlDoc.DocumentElement.SetAttribute("xmlns:opds", "http://opds-spec.org/2010/catalog");
                xmlDoc.DocumentElement.SetAttribute("xmlns:app", "http://www.w3.org/2007/app");
            }
        }

        private List<XElement> GetEntries(List<Entry> entries)
        {
            List<XElement> elements = new List<XElement>();
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    elements.Add(new XElement("entry",
                                              new XElement("Title", entry.Title),
                                              new XElement("id", entry.Id),
                                              new XElement("updated", entry.Updated),
                                              new XElement("author",
                                                           new XElement("name", entry.AuthourName)),
                                              new XElement("dcterms:issued", entry.Updated.ToShortDateString()),
                                              GetSummary(entry),
                                              GetCoverImage(entry),
                                              GetEditionUrl(entry),
                                              GetAlternateEdition(entry)
                                     ));
                }
            }
            return elements;
        }

        private XElement GetAlternateEdition(Entry entry)
        {
            return new XElement("a");
        }

        private XElement GetEditionUrl(Entry entry)
        {
            return new XElement("a");
        }

        private XElement GetCoverImage(Entry entry)
        {
            return new XElement("a");
        }

        private XElement GetSummary(Entry entry)
        {
            var summary = new XElement("summary",entry.Summary);
            summary.SetAttributeValue("type", "text");
            return summary;
        }
    }
}
