using System;
using System.Xml;
using Moq;
using NUnit.Framework;
using Umbraco.Framework;
using Umbraco.Pugpig.Core.Controllers;
using Umbraco.Pugpig.Core.Interfaces;
using Umbraco.Pugpig.Core.Models;

namespace Umbraco.Pugpig.Core.Tests
{
    [TestFixture]
    public class ControllerTests
    {
        private Mock<IAbstractRequest> m_requestFactory;
        private Mock<IPugpigRepository> m_pugpigRepository;

        [SetUp]
        public void Setup()
        {
            m_pugpigRepository = new Mock<IPugpigRepository>();
            m_pugpigRepository.Setup(x => x.CreateEditionList(It.IsAny<string>())).Returns(new Feed());
        }

        public void PupgpigController_RenderEditionList()
        {
            //var controller = new PugpigController(m_pugpigRepository.Object, m_requestFactory.Object);

            //XmlDocument editions = controller.Editions(It.IsAny<string>());

            //Assert.IsNotNull(editions);
        }

        public void PupgpigController_RenderEditionDetail()
        {

        }
    }
}
