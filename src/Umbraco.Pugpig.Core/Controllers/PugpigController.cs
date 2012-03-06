using System.Web.Mvc;
using System.Xml;
using Umbraco.Cms.Web;
using Umbraco.Cms.Web.Context;
using Umbraco.Cms.Web.Mvc.Controllers;
using Umbraco.Cms.Web.Surface;
using Umbraco.Framework;
using Umbraco.Framework.Diagnostics;
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
        private readonly IRoutableRequestContext m_routableRequest;
        private readonly IRenderModelFactory m_renderModelFactory;
        private IAbstractRequest m_abstractRequest;

        public PugpigSurfaceController()
        {
            
        }

        public  ActionResult Index()
        {
            return View();
        }

        public PugpigSurfaceController(IAbstractRequest abstractRequest, 
            IPugpigRepository pugpigRepository, IRoutableRequestContext routableRequest,IRenderModelFactory renderModelFactory)
        {
            m_pugpigRepository = pugpigRepository;
            m_routableRequest = routableRequest;
            m_renderModelFactory = renderModelFactory;
            m_abstractRequest = abstractRequest;
        }

        public XmlResult Editions(string publicationName)
        {
            UmbracoHelper umbracoHelper = new UmbracoHelper(this.ControllerContext,m_routableRequest, m_renderModelFactory);

            LogHelper.TraceIfEnabled<PugpigSurfaceController>("The edition passed into the controller was {0}.", () => publicationName);       
            CreaterFormatter(publicationName);
            return new XmlResult(m_xmlFormatter.GenerateXml(m_pugpigRepository.CreateEditionList(publicationName, umbracoHelper)));
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
