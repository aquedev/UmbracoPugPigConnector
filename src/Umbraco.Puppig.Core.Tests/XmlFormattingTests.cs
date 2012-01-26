using System;
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
        private Mock<IFeedSettings> m_settings = new Mock<IFeedSettings>();

        [SetUp]
        public void Setup()
        {
            m_settings.SetupGet(x => x.FeedId).Returns("com.umbraco.aqueduct");
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
            var feed = GetFeed(0);

            XmlDocument doc = new XmlFormatter(m_settings.Object).GenerateXml(feed);

            Assert.IsNotNull(doc);
            var reader = new XmlTextReader(new StringReader(doc.OuterXml));
            using (XmlReader expectedXml = LoadDataFeed("1Edition"), actualXml = reader)
            {
                Assert.That(actualXml, IsXml.EquivalentTo(expectedXml).IgnoreSequenceOrder);
            }
        }

        private Feed GetFeed(int numberOfEntries)
        {
            var feed = new Feed();
            feed.Title = "All Editions";
            feed.LastUpdated = new DateTime(2011, 8, 8, 15, 0, 0, 0);
            return feed;
        }

        private XmlReader LoadDataFeed(string name)
        {
            var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("Umbraco.Pugpig.Core.Tests.Data.{0}.xml", name)));
            return XmlReader.Create(reader);
        }
    }
}
