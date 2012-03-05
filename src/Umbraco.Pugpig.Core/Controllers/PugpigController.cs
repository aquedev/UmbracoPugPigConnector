using System.Web.Mvc;
using System.Xml;
using Umbraco.Cms.Web.Mvc.Controllers;
using Umbraco.Cms.Web.Surface;
using Umbraco.Framework;
using Umbraco.Pugpig.Core.Interfaces;
using Umbraco.Pugpig.Core.Settings;

namespace Umbraco.Pugpig.Core.Controllers
{
    [Surface("98625300-6DF0-41AF-A432-83BD0232815C")]
    [DemandsDependencies(typeof(PugpigBuilder))]
    public class PugpigSurfaceController : SurfaceController
    {
        private IXmlFormatter m_xmlFormatter;
        private IPugpigRepository m_pugpigRepository;
        private IAbstractRequest m_abstractRequest;

        public PugpigSurfaceController()
        {
            
        }

        public  ActionResult Index()
        {
            return View();
        }

        public PugpigSurfaceController(IAbstractRequest abstractRequest, IPugpigRepository pugpigRepository)
        {
            m_pugpigRepository = pugpigRepository;
            m_abstractRequest = abstractRequest;
        }

        public XmlDocument Editions(string publicationName)
        {
            CreaterFormatter(publicationName);
            return m_xmlFormatter.GenerateXml(m_pugpigRepository.CreateEditionList(publicationName));
        }

        private void CreaterFormatter(string publicationName)
        {
            var feedSettings = new FeedSettings();
            feedSettings.BaseUrl = m_abstractRequest.GetBaseUrl();
            feedSettings.PublicationName = publicationName;
            m_xmlFormatter = new XmlFormatter(feedSettings);
        }
    }
}
