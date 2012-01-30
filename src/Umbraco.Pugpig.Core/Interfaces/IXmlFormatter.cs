using System.Xml;
using Umbraco.Pugpig.Core.Models;

namespace Umbraco.Pugpig.Core.Interfaces
{
    public interface IXmlFormatter
    {
        XmlDocument GenerateXml(Feed feed);
    }
}