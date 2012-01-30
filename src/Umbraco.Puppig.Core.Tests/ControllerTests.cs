using System;
using System.Xml;
using Moq;
using NUnit.Framework;
using Umbraco.Pugpig.Core.Interfaces;
using Umbraco.Pugpig.Core.Models;
using Umbraco.Pugpig.Web.Controllers;

namespace Umbraco.Pugpig.Core.Tests
{
    [TestFixture]
    public class ControllerTests
    {
        private Mock<IXmlFormatter> m_xmlFormatter;
        private Mock<IPugpigRepository> m_pugpigRepository;

        [SetUp]
        public void Setup()
        {
            m_xmlFormatter = new Mock<IXmlFormatter>();
            m_pugpigRepository.Setup(x => x.CreateEditionList(It.IsAny<Guid>())).Returns(new Feed());
        }

        public void PupgpigController_RenderEditionList()
        {
            var controller = new PugpigController(m_xmlFormatter.Object, m_pugpigRepository.Object);

            XmlDocument editions = controller.Editions(It.IsAny<Guid>());

            Assert.IsNotNull(editions);
        }

        public void PupgpigController_RenderEditionDetail()
        {

        }
    }
}
