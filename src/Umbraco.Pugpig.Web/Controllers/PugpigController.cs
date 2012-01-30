using System;
using System.Xml;
using Umbraco.Cms.Web.Mvc.Controllers;
using Umbraco.Pugpig.Core.Interfaces;

namespace Umbraco.Pugpig.Web.Controllers
{
    public class PugpigController : UmbracoController
    {
        private readonly IXmlFormatter m_xmlFormatter;
        private readonly IPugpigRepository m_pugpigRepository;

        public PugpigController(IXmlFormatter xmlFormatter, IPugpigRepository pugpigRepository)
        {
            m_xmlFormatter = xmlFormatter;
            m_pugpigRepository = pugpigRepository;
        }

        public XmlDocument Editions(Guid id)
        {
            return m_xmlFormatter.GenerateXml(m_pugpigRepository.CreateEditionList(id));
        }
    }
}
