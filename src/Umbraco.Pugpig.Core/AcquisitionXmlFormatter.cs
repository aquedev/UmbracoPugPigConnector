using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Umbraco.Pugpig.Core.Interfaces;
using Umbraco.Pugpig.Core.Models;

namespace Umbraco.Pugpig.Core
{
    public class AcquisitionXmlFormatter : IAcquisitionXmlFormatter
    {
        private readonly IBookSettings m_feedInfo;

        public AcquisitionXmlFormatter(IBookSettings feedInfo)
        {
            m_feedInfo = feedInfo;
        }

        public XmlDocument GenerateXml(Book feed)
        {
            var element = new XElement("feed",
                                       new XElement("id", m_feedInfo.BookName),
                                       GetLinkElement(m_feedInfo.BookName),
                                       new XElement("title", feed.Title),
                                       new XElement("updated", feed.LastUpdated.ToString("yyyy-MM-ddTH:mm:sszzz")),
                                       GetAuthour(m_feedInfo.AuthourName),
                                       GetEntries(feed.Pages));

            XmlReader xmlReader = element.CreateReader();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlReader);

            SetNamespaces(xmlDoc);

            return xmlDoc;
        }

        private object GetAuthour(string authourName)
        {
            return new XElement("author",
                                new XElement("name", authourName));
        }

        private XElement GetLinkElement(string bookName)
        {
            XElement link = new XElement("link");
            link.SetAttributeValue("rel", "self");
            link.SetAttributeValue("type", "application/atom+xml;profile=opds-catalog;kind=acquisition");
            link.SetAttributeValue("href", String.Concat(m_feedInfo.BaseUrl, "/umbraco/pugpig/pugpigSurface/Acquisition/", bookName));
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

        private List<XElement> GetEntries(List<Page> entries)
        {
            List<XElement> elements = new List<XElement>();
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    XNamespace xNamespace = "http://purl.org/dc/terms/";
                    elements.Add(new XElement("entry",
                                              new XElement("title", entry.Title),
                                              new XElement("id", String.Concat("com.umbraco.edition.",entry.Id)),
                                              new XElement("updated", entry.Updated.ToString("yyyy-MM-ddTH:mm:sszzz")),
                                              new XElement("published", entry.Updated.ToString("yyyy-MM-ddTH:mm:sszzz")),
                                              new XElement("summary"),
                                              GetAlternateEdition(entry),
                                              GetRelatedUrl(entry)
                                              
                                     ));
                }
            }
            return elements;
        }

        private XElement GetAlternateEdition(Page entry)
        {
            var editionUrl = new XElement("link");
            editionUrl.SetAttributeValue("rel", "alternate");
            editionUrl.SetAttributeValue("type", "text/html");
            editionUrl.SetAttributeValue("href", entry.PageUrl);
            return editionUrl;
        }

        private XElement GetRelatedUrl(Page entry)
        {
            var editionUrl = new XElement("link");
            editionUrl.SetAttributeValue("rel", "http://opds-spec.org/acquisition");
            editionUrl.SetAttributeValue("type", "application/atom+xml");
            editionUrl.SetAttributeValue("href", String.Format("{0}/umbraco/pugpig/pugpigSurface/Manifest?bookName={1}",m_feedInfo.BaseUrl,m_feedInfo.BookName));
            return editionUrl;
        }
    }
}
