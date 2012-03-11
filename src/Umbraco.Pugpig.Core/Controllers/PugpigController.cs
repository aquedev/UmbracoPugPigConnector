using System.Collections.Generic;
using System.Web.Mvc;
using Umbraco.Cms.Web;
using Umbraco.Cms.Web.Context;
using Umbraco.Cms.Web.Surface;
using Umbraco.Framework;
using Umbraco.Pugpig.Core.Interfaces;
using Umbraco.Pugpig.Core.Settings;

namespace Umbraco.Pugpig.Core.Controllers
{
    public class PublicationSumaryModel
    {
        public string FeedUrl { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }

    [Surface("98625300-6DF0-41AF-A432-83BD0232815C")]
    [DemandsDependencies(typeof(PugpigBuilder))]
    public class PugpigSurfaceController : SurfaceController
    {
        private IEditionXmlFormatter m_editionXmlFormatter;
        private IAcquisitionXmlFormatter m_acquisitionXmlFormatter;
        private IPugpigRepository m_pugpigRepository;
        private readonly IRoutableRequestContext m_routableRequest;
        private readonly IRenderModelFactory m_renderModelFactory;
        private IAbstractRequest m_abstractRequest;

        public PugpigSurfaceController()
        {
            
        }

        public  ActionResult Index()
        {
            UmbracoHelper umbracoHelper = new UmbracoHelper(ControllerContext, m_routableRequest, m_renderModelFactory);
            List<PublicationSumaryModel> allPublications = m_pugpigRepository.GetAllPublications(umbracoHelper);
            return View(allPublications);
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
            UmbracoHelper umbracoHelper = new UmbracoHelper(ControllerContext,m_routableRequest, m_renderModelFactory);
            CreaterEditionFormatter(publicationName);
            return new XmlResult(m_editionXmlFormatter.GenerateXml(m_pugpigRepository.CreateEditionList(publicationName, umbracoHelper)));
        }

        public XmlResult Acquisition(string edition, string publicationName)
        {
            UmbracoHelper umbracoHelper = new UmbracoHelper(ControllerContext, m_routableRequest, m_renderModelFactory);
            CreaterAcquisitionFormatter(edition);
            return new XmlResult(m_acquisitionXmlFormatter.GenerateXml(m_pugpigRepository.CreateBookList(edition,publicationName, umbracoHelper)));

        }

        private void CreaterAcquisitionFormatter(string edition)
        {
            var feedSettings = new BookSettings();
            feedSettings.BaseUrl = m_abstractRequest.GetBaseUrl();
            feedSettings.BookName = edition;
            m_acquisitionXmlFormatter = new AcquisitionXmlFormatter(feedSettings);
        }

        private void CreaterEditionFormatter(string publicationName)
        {
            var feedSettings = new FeedSettings();
            feedSettings.BaseUrl = m_abstractRequest.GetBaseUrl();
            feedSettings.PublicationName = publicationName;
            m_editionXmlFormatter = new EditionXmlFormatter(feedSettings);
        }
    }
}
