using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using NUnit.Framework;
using Jolt.Testing.Assertions.NUnit.SyntaxHelpers;
using Umbraco.Pugpig.Core;
using Umbraco.Pugpig.Core.Models;

namespace Umbraco.Puppig.Core.Tests
{
    [TestFixture]
    public class XmlFormattingTests
    {
        [Test]
        public void GenerateXML_GivenNoEditions_ProducesValidFeed()
        {
            XmlDocument doc = new XmlFormatter().GenerateXml(new List<Entry>());
            Assert.IsNotNull(doc);
            var reader = new XmlTextReader(new StringReader(doc.OuterXml));
            using (XmlReader expectedXml = LoadDataFeed("EmptyFeed"), actualXml = reader)
            {
                Assert.That(actualXml, IsXml.EquivalentTo(expectedXml).IgnoreAttributes.IgnoreSequenceOrder);
            }
        }

        private XmlReader LoadDataFeed(string name)
        {
            var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("Umbraco.Pugpig.Core.Tests.Data.{0}.xml", name)));
            return XmlReader.Create(reader);
        }
    }
}
