using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Umbraco.Pugpig.Core.Models;

namespace Umbraco.Pugpig.Core
{
    public class XmlFormatter
    {
        public XmlDocument GenerateXml(List<Entry> entries)
        {
            var element = new XElement("NHSFeed",
                                       new XElement("Title", "British Red Cross: UK"),
                                       new XElement("Link", "http://www.redcross.org.uk/handlers/EveryDayFirstAid"),
                                       new XElement("Description",
                                                    "An XML feed containing steps to treat all everyday first aid conditions on the British Red Cross website"),
                                       GetEntries(entries));


            XmlReader xmlReader = element.CreateReader();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlReader);
            return xmlDoc;
        }

        private object GetEntries(List<Entry> entries)
        {
            throw new NotImplementedException();
        }
    }
}
