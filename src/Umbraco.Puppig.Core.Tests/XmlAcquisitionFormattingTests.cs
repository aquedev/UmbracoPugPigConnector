using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Moq;
using NUnit.Framework;
using Jolt.Testing.Assertions.NUnit.SyntaxHelpers;
using Umbraco.Pugpig.Core.Interfaces;
using Umbraco.Pugpig.Core.Models;

namespace Umbraco.Pugpig.Core.Tests
{
    [TestFixture]
    public class XmlAcquisitionFormattingTests
    {
        private readonly Mock<IBookSettings> m_settings = new Mock<IBookSettings>();

        [SetUp]
        public void Setup()
        {
            m_settings.SetupGet(x => x.BookName).Returns("com.umbraco.aqueduct");
            m_settings.SetupGet(x => x.BaseUrl).Returns("umbraco.local");
            m_settings.SetupGet(x => x.AuthourName).Returns("Lee Cook");
        }

        [Test]
        public void GenerateXML_GivenOnePage_ProducesValidFeed()
        {
            var feed = GetBook(1);

            XmlDocument doc = new AcquisitionXmlFormatter(m_settings.Object).GenerateXml(feed);

            Assert.IsNotNull(doc);
            var reader = new XmlTextReader(new StringReader(doc.OuterXml));
            using (XmlReader expectedXml = LoadDataFeed("1PageBookExample"), actualXml = reader)
            {
                Assert.That(actualXml, IsXml.EquivalentTo(expectedXml).IgnoreSequenceOrder);
            }
        }

        [TestCase(2)]
        [TestCase(3)]
        [Test]
        public void GenerateXML_GivenMultipleEditions_ProducesValidFeed(int NumberOfEditions)
        {
            var feed = GetBook(NumberOfEditions);

            XmlDocument doc = new AcquisitionXmlFormatter(m_settings.Object).GenerateXml(feed);

            Assert.IsNotNull(doc);
            var reader = new XmlTextReader(new StringReader(doc.OuterXml));
            using (XmlReader expectedXml = LoadDataFeed(String.Concat(NumberOfEditions, "PageBookExample")), actualXml = reader)
            {
                Assert.That(actualXml, IsXml.EquivalentTo(expectedXml).IgnoreSequenceOrder);
            }
        }

        private Book GetBook(int numberOfEntries)
        {
            var feed = new Book();
            feed.Title = "All Editions";
            feed.LastUpdated = new DateTime(2011, 8, 8, 15, 0, 0, 0);
            feed.Pages = new List<Page>();
            int i = 0;
            while(i < numberOfEntries)
            {
                feed.Pages.Add(new Page
                                     {
                                         Id = (i + 1).ToString(), 
                                         Title = "My First Edition" + (i + 1), 
                                         Updated = new DateTime(2011, 8, 8, 15, 0, 0, 0), 
                                         PageUrl = "PageUrl"
                                     });
                i++;
            }
            return feed;
        }

        private XmlReader LoadDataFeed(string name)
        {
            var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("Umbraco.Pugpig.Core.Tests.Data.Books.{0}.xml", name)));
            return XmlReader.Create(reader);
        }
    }
}
