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
    public class XmlFormattingTests
    {
        private readonly Mock<IFeedSettings> m_settings = new Mock<IFeedSettings>();

        [SetUp]
        public void Setup()
        {
            m_settings.SetupGet(x => x.PublicationName).Returns("com.umbraco.aqueduct");
            m_settings.SetupGet(x => x.BaseUrl).Returns("umbraco.local");
        }

        [Test]
        public void GenerateXML_GivenNoEditions_ProducesValidFeed()
        {
            var feed = GetFeed(0);

            XmlDocument doc = new XmlFormatter(m_settings.Object).GenerateXml(feed);

            Assert.IsNotNull(doc);
            var reader = new XmlTextReader(new StringReader(doc.OuterXml));
            using (XmlReader expectedXml = LoadDataFeed("EmptyFeed"), actualXml = reader)
            {
                Assert.That(actualXml, IsXml.EquivalentTo(expectedXml).IgnoreSequenceOrder);
            }
        }

        [Test]
        public void GenerateXML_GivenOneEdition_ProducesValidFeed()
        {
            var feed = GetFeed(1);

            XmlDocument doc = new XmlFormatter(m_settings.Object).GenerateXml(feed);

            Assert.IsNotNull(doc);
            var reader = new XmlTextReader(new StringReader(doc.OuterXml));
            using (XmlReader expectedXml = LoadDataFeed("1Edition"), actualXml = reader)
            {
                Assert.That(actualXml, IsXml.EquivalentTo(expectedXml).IgnoreSequenceOrder);
            }
        }

        [TestCase(2)]
        [TestCase(3)]
        [Test]
        public void GenerateXML_GivenMultipleEditions_ProducesValidFeed(int NumberOfEditions)
        {
            var feed = GetFeed(NumberOfEditions);

            XmlDocument doc = new XmlFormatter(m_settings.Object).GenerateXml(feed);

            Assert.IsNotNull(doc);
            var reader = new XmlTextReader(new StringReader(doc.OuterXml));
            using (XmlReader expectedXml = LoadDataFeed(String.Concat(NumberOfEditions,"Edition")), actualXml = reader)
            {
                Assert.That(actualXml, IsXml.EquivalentTo(expectedXml).IgnoreSequenceOrder);
            }
        }

        private Feed GetFeed(int numberOfEntries)
        {
            var feed = new Feed();
            feed.Title = "All Editions";
            feed.LastUpdated = new DateTime(2011, 8, 8, 15, 0, 0, 0);
            feed.Entries = new List<Entry>();
            int i = 0;
            while(i < numberOfEntries)
            {
                feed.Entries.Add(new Entry
                                     {
                                         AuthourName = "Lee Cook", 
                                         Id = i + 1.ToString(), Summary = "summary", 
                                         Title = "My First Edition" + (i + 1), 
                                         Updated = new DateTime(2011, 8, 8, 15, 0, 0, 0),
                                         Image = new Image { Url = "img/cover.jpg" }
                                     });
                i++;
            }
            return feed;
        }

        private XmlReader LoadDataFeed(string name)
        {
            var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("Umbraco.Pugpig.Core.Tests.Data.{0}.xml", name)));
            return XmlReader.Create(reader);
        }
    }
}
