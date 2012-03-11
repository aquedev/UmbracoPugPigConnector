using System;
using System.Web.Mvc;
using Umbraco.Cms.Web.Context;
using Umbraco.Cms.Web.DependencyManagement;
using Umbraco.Cms.Web.Editors;
using Umbraco.Cms.Web.Model.BackOffice.Editors;
using Umbraco.Framework;
using Umbraco.Framework.Context;
using Umbraco.Framework.Persistence;
using Umbraco.Hive;
using Umbraco.Pugpig.Core.Installers;

namespace Umbraco.Pugpig.Core.Controllers
{
    [Editor("98615300-6DF0-41AF-A432-83BD0232815F")]
    public class InitContentEditorController : StandardEditorController
    {
        private PugpigDataset m_pugpigDataSet;
        public IHiveManager Hive { get; set; }
        public DocumentTypeEditorModel DocumentTypeEditorModel { get; set; }
        public IFrameworkContext FrameworkContext { get; set; }

        public InitContentEditorController(IBackOfficeRequestContext requestContext)
            : base(requestContext)
        {
            m_pugpigDataSet = new PugpigDataset(new PropertyEditorFactory(),requestContext.Application.Hive.FrameworkContext, new DefaultAttributeTypeRegistry());
            Hive = requestContext.Application.Hive;
            FrameworkContext = requestContext.Application.Hive.FrameworkContext;
        }

         public ActionResult InstallPugpigData()
         {
             m_pugpigDataSet.InstallDevDataset(Hive, FrameworkContext);
             return View();

         }


        public override ActionResult Edit(HiveId? id)
        {
            throw new NotImplementedException();
        }
    }
}