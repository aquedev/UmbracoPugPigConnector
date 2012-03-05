using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Cms.Web.Context;
using Umbraco.Cms.Web.DependencyManagement;
using Umbraco.Cms.Web.Editors;
using Umbraco.Cms.Web.Model.BackOffice.Editors;
using Umbraco.Cms.Web.Mvc.Controllers;
using Umbraco.Cms.Web.Surface;
using Umbraco.Framework;
using Umbraco.Framework.Context;
using Umbraco.Framework.Persistence;
using Umbraco.Framework.Persistence.Model;
using Umbraco.Framework.Persistence.Model.Attribution.MetaData;
using Umbraco.Framework.Persistence.Model.Constants;
using Umbraco.Framework.Persistence.Model.Constants.AttributeDefinitions;
using Umbraco.Framework.Persistence.Model.Versioning;
using Umbraco.Hive;
using Umbraco.Hive.RepositoryTypes;
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

        //public InitContentSurfaceController(IHiveManager hiveManager)
        //{
           
        //}
         public ActionResult InstallPugpigData()
         {
             m_pugpigDataSet.InstallDevDataset(Hive, FrameworkContext);
             return Content("Done");
         }

        public ActionResult Index(string documentTypeAlias, string nodeName, string parentId)
        {
            DocumentTypeEditorModel = GetDocumentType(documentTypeAlias);

            var parentHiveId = string.IsNullOrEmpty(parentId) ? FixedHiveIds.ContentVirtualRoot : new HiveId(new Guid(parentId));

            var nameProperty = SetProperty(NodeNameAttributeDefinition.AliasValue, new Dictionary<string, object> { { "Name", nodeName } });

            var shortDescriptionProperty = SetProperty("ShortDescription", "bla");
            var fullDescriptionProperty = SetProperty("FullDescription", 15);

            var properties = new HashSet<ContentProperty> { nameProperty, shortDescriptionProperty, fullDescriptionProperty };

            var contentNode = new ContentEditorModel
            {
                DocumentTypeId = DocumentTypeEditorModel.Id,
                DocumentTypeAlias = documentTypeAlias,
                ParentId = parentHiveId,
                Properties = new HashSet<ContentProperty>(properties)
            };

            var contentRepository = new List<ContentEditorModel> { contentNode };

            using (var writer = Hive.OpenWriter<IContentStore>())
            {
                var mappedCollection = FrameworkContext.TypeMappers.Map<IEnumerable<ContentEditorModel>, IEnumerable<Revision<TypedEntity>>>(contentRepository).ToArray();
                mappedCollection.ForEach(x => x.MetaData.StatusType = FixedStatusTypes.Published);

                writer.Repositories.Revisions.AddOrUpdate(mappedCollection);
                writer.Complete();
            }

            return Content("Done");
        }

        private ContentProperty SetProperty(string propertyAlias, object propertyValue)
        {
            var docTypeProperty = DocumentTypeEditorModel.Properties.Single(x => x.Alias == propertyAlias);

            //NOTE: If the Umbraco API changes (an extra overload is added to new ContentProperty), this might break
            var dictionary = propertyValue as IDictionary<string, object> ?? new Dictionary<string, object> { { "Value", propertyValue } };

            var property = new ContentProperty(docTypeProperty.Id, docTypeProperty, dictionary)
            {
                Alias = propertyAlias,
                TabId = DocumentTypeEditorModel.DefinedTabs.Single(x => x.Alias == FixedGroupDefinitions.GeneralGroup.Alias).Id
            };

            return property;
        }

        private DocumentTypeEditorModel GetDocumentType(string documentTypeAlias)
        {
            EntitySchema categorySchema;
            using (var uow = Hive.OpenReader<IContentStore>())
            {
                categorySchema = uow.Repositories.Schemas.GetAll<EntitySchema>().Single(x => x.Alias == documentTypeAlias);
            }

            return FrameworkContext.TypeMappers.Map<DocumentTypeEditorModel>(categorySchema);
        }

        public override ActionResult Edit(HiveId? id)
        {
            throw new NotImplementedException();
        }
    }
}