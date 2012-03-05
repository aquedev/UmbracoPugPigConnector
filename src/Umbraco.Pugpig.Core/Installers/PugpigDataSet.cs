using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Umbraco.Cms.Web;
using Umbraco.Cms.Web.Model;
using Umbraco.Cms.Web.Model.BackOffice.Editors;
using Umbraco.Framework.Diagnostics;
using Umbraco.Framework.Persistence;
using Umbraco.Framework;
using Umbraco.Framework.Context;
using Umbraco.Framework.Persistence.Model;
using Umbraco.Framework.Persistence.Model.Associations;
using Umbraco.Framework.Persistence.Model.Attribution.MetaData;
using Umbraco.Framework.Persistence.Model.Constants;
using Umbraco.Framework.Persistence.Model.Constants.AttributeDefinitions;
using Umbraco.Framework.Persistence.Model.Versioning;
using Umbraco.Hive;
using Umbraco.Hive.RepositoryTypes;

namespace Umbraco.Pugpig.Core.Installers
{
    /// <summary>
    /// USED ONLY FOR DEMO DATA!!!
    /// </summary>
    /// <remarks>
    /// Is most likely NOT thread safe
    /// </remarks>
    public class PugpigDataset
    {
        private readonly IFrameworkContext _frameworkContext;


        /// <summary>
        /// creates a test repository of UmbracoNodes
        /// </summary>
        private List<ContentEditorModel> _testRepository;
        private List<DataType> _dataTypes;

        private readonly XDocument _nodeData;
        private IEnumerable<DocumentTypeEditorModel> _docTypes;
        private IEnumerable<TemplateFile> _templates;
        // private Dictionary<int, string> _creators;


        #region DevDataset

        public PugpigDataset(IPropertyEditorFactory propertyEditorFactory, IFrameworkContext frameworkContext, IAttributeTypeRegistry attributeTypeRegistry)
        {
            _frameworkContext = frameworkContext;
            PropertyEditorFactory = propertyEditorFactory;
            //InitCreators();


            // Anthony's code
            var helper = new UmbracoXmlImportHelper(attributeTypeRegistry, propertyEditorFactory);
            _templates = helper.InitTemplates();
            _dataTypes = helper.InitDataTypes(_frameworkContext);
            _docTypes = helper.InitDocTypes();



            // get content node xml
            _nodeData = XDocument.Parse(Files.umbraco);

        }

        #endregion

        #region InstallDevDataset

        /// <summary>
        /// TEMPORARY method to install all data required for dev data set excluding all of the core data
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="framework"></param>
        internal void InstallDevDataset(IHiveManager manager, IFrameworkContext framework)
        {
            //LogHelper.Error<PugpigInstallTask>(
                 //      string.Format("Inside the dataset class"), new Exception());

            //a list of all the schemas we've added
            var schemas = new List<EntitySchema>();

            using (var writer = manager.OpenWriter<IContentStore>())
            {
                //create all of the document type's and their associated tabs first
                //foreach (var d in _devDataSet.ContentData.Select(x => x.DocumentType).DistinctBy(x => x.Id))
                foreach (var d in DocTypes)
                {

                    var schema = new EntitySchema(d.Alias, d.Name);
                    schema.Id = d.Id;
                    schema.AttributeGroups.AddRange(
                        framework.TypeMappers.Map<IEnumerable<Tab>, IEnumerable<AttributeGroup>>(
                            d.DefinedTabs));
                    writer.Repositories.Schemas.AddOrUpdate(schema);
                    schemas.Add(schema);

                    foreach (var parentId in d.InheritFrom.Where(x => x.Selected).Select(x => HiveId.Parse(x.Value)))
                        writer.Repositories.AddRelation(new Relation(FixedRelationTypes.DefaultRelationType, parentId, d.Id));
                }
                writer.Complete();
            }

            using (var writer = manager.OpenWriter<IContentStore>())
            {
                //now we can hopefully just map the schema and re-save it so it maps all properties
                //foreach (var d in _devDataSet.ContentData.Select(x => x.DocumentType).DistinctBy(x => x.Id))
                foreach (var d in DocTypes)
                {
                    var schema = framework.TypeMappers.Map<DocumentTypeEditorModel, EntitySchema>(d);
                    writer.Repositories.Schemas.AddOrUpdate(schema);
                }
                writer.Complete();
            }

            using (var writer = manager.OpenWriter<IContentStore>())
            {
                //now just map the entire content entities and persist them, since the attribution definitions and attribution 
                //groups are created these should just map up to the entities in the database.

                var mappedCollection = framework
                    .TypeMappers.Map<IEnumerable<ContentEditorModel>, IEnumerable<Revision<TypedEntity>>>(ContentData)
                    .ToArray();
                mappedCollection.ForEach(x => x.MetaData.StatusType = FixedStatusTypes.Published);

                //var allAttribTypes = AllAttribTypes(mappedCollection);

                writer.Repositories.Revisions.AddOrUpdate(mappedCollection);

                writer.Complete();
            }

            ////now that the data is in there, we need to setup some structure... probably a nicer way to do this but whatevs... its just for testing
            //using (var writer = mappingGroup.CreateReadWriteUnitOfWork())
            //{
            //    var homeSchema = writer.ReadWriteRepository.GetEntity<EntitySchema>(HiveId.ConvertIntToGuid(1045));
            //    var contentSchema = writer.ReadWriteRepository.GetEntity<EntitySchema>(HiveId.ConvertIntToGuid(1045));
            //    var faqContainerSchema = writer.ReadWriteRepository.GetEntity<EntitySchema>(HiveId.ConvertIntToGuid(1055));
            //    var faqCatSchema = writer.ReadWriteRepository.GetEntity<EntitySchema>(HiveId.ConvertIntToGuid(1056));
            //    var faqSchema = writer.ReadWriteRepository.GetEntity<EntitySchema>(HiveId.ConvertIntToGuid(1057));

            //}
        }

        #endregion




        #region PropertyEditorFactory
        /// <summary>
        /// Because we need to have model properties consistent across postbacks, and model property IDs are Guids which are not stored
        /// in the XML, we need to create a map so they remain consistent.
        /// </summary>
        /// <remarks>
        /// To create the map, we'll map the model ID + the model property name to a GUID
        /// </remarks>
        private static List<KeyValuePair<KeyValuePair<int, string>, Guid>> _nodePropertyIdMap;

        public IPropertyEditorFactory PropertyEditorFactory { get; private set; }

        public XDocument XmlData
        {
            get
            {
                return _nodeData;
            }
        }

        #endregion



        #region public properties

        /// <summary>
        /// Returns a test repository for querying/updating
        /// </summary>
        public IEnumerable<ContentEditorModel> ContentData
        {
            get
            {
                if (_testRepository == null)
                {
                    InitRepository();
                }
                return _testRepository;
            }
        }

        /// <summary>
        /// Returns a set of data types to test against
        /// </summary>
        public IEnumerable<DataType> DataTypes
        {
            get
            {
                return _dataTypes;
            }
        }

        public IEnumerable<DocumentTypeEditorModel> DocTypes
        {
            get { return _docTypes; }
        }

        public IEnumerable<TemplateFile> Templates
        {
            get { return _templates; }
        }

        #endregion



        #region GetNodeProperties
        /// <summary>
        /// Get node properties content from xml
        /// </summary>
        /// <param name="nodeXml"></param>
        /// <param name="selectedTemplateId"></param>
        /// <returns></returns>
        private HashSet<ContentProperty> GetNodeProperties(XElement nodeXml, HiveId selectedTemplateId)
        {
            var customProperties = new List<ContentProperty>();
            //var tabIds = _docTypes.SelectMany(tabs => tabs.DefinedTabs).Select(x => x.Id).ToList();

            //get the corresponding doc type for this node
            var docType = _docTypes.SingleOrDefault(x => x.Alias.ToLowerInvariant() == nodeXml.Name.LocalName.ToLowerInvariant());
            if (docType == null)
            {
                // doctype doesnt exist...but should we be checking this here?

            }

            //add node name property
            var nodeName = CreateNodeNameContentProperty(docType, nodeXml);
            customProperties.Add(nodeName);

            //add selected template (empty) property
            var selectedTemplate = CreateSelectedTemplateContentProperty(docType, selectedTemplateId);
            customProperties.Add(selectedTemplate);


            var propertiesXml = nodeXml.Elements().Where(e => e.Attribute("isDoc") == null);

            foreach (var e in propertiesXml)
            {
                //Assigning the doc type properties is completely arbitrary here, all I'm doing is 
                //aligning a DocumentTypeProperty that contains the DataType that I want to render
                ContentProperty contentProperty;

                //get property by alias
                DocumentTypeProperty docTypeProperty = docType.Properties.SingleOrDefault(x => x.Alias == e.Name.LocalName);

                if (docTypeProperty == null)
                {
                    // content item is not in doctype schema
                    continue;
                }

                // if it's datatype alias is an upload, split the string and get the values
                if (docTypeProperty.DataType.Alias.ToLowerInvariant() == "uploader")
                {
                    var values = e.Value.Split(',');
                    contentProperty = new ContentProperty((HiveId)GetNodeProperty(e), docTypeProperty, new Dictionary<string, object> { { "MediaId", values[0] }, { "Value", values[1] } });
                }
                else
                {
                    contentProperty = new ContentProperty((HiveId)GetNodeProperty(e), docTypeProperty, e.Value);
                }

                //need to set the data type model for this property
                contentProperty.Alias = e.Name.LocalName;
                contentProperty.Name = e.Name.LocalName;

                //add to a random tab - wtf is this?
                //int currTab = 0;
                //contentProperty.TabId = tabIds[currTab];

                // get property tab?
                contentProperty.TabId = docTypeProperty.TabId;

                customProperties.Add(contentProperty);
            }

            return new HashSet<ContentProperty>(customProperties);
        }


        /// <summary>
        /// Create selected template content property.
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="selectedTemplateId"> </param>
        /// <returns></returns>
        private ContentProperty CreateSelectedTemplateContentProperty(DocumentTypeEditorModel docType, HiveId selectedTemplateId)
        {
            var prop = new ContentProperty((HiveId)Guid.NewGuid(),
                                                       docType.Properties.Single(x => x.Alias == SelectedTemplateAttributeDefinition.AliasValue),
                                                       selectedTemplateId.IsNullValueOrEmpty() ? new Dictionary<string, object>() : new Dictionary<string, object> { { "TemplateId", selectedTemplateId.ToString() } })
            {
                Name = SelectedTemplateAttributeDefinition.AliasValue,
                Alias = SelectedTemplateAttributeDefinition.AliasValue,
                TabId = docType.DefinedTabs.Single(x => x.Alias == FixedGroupDefinitions.GeneralGroup.Alias).Id
            };
            return prop;
        }


        /// <summary>
        /// Create selected template content property.
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private ContentProperty CreateNodeNameContentProperty(DocumentTypeEditorModel docType, XElement nodeXml)
        {
            var prop = new ContentProperty((HiveId)Guid.NewGuid(),
                                                   docType.Properties.Single(x => x.Alias == NodeNameAttributeDefinition.AliasValue),
                                                   new Dictionary<string, object> { { "Name", (string)nodeXml.Attribute("nodeName") } })
            {
                Name = NodeNameAttributeDefinition.AliasValue,
                Alias = NodeNameAttributeDefinition.AliasValue,
                TabId = docType.DefinedTabs.Single(x => x.Alias == FixedGroupDefinitions.GeneralGroup.Alias).Id

            };

            return prop;
        }

        #endregion





        #region InitRepository

        private void InitRepository()
        {
            _testRepository = new List<ContentEditorModel>();

            var docsXml = XmlData.Root.Descendants().Where(x => x.Attribute("isDoc") != null);

            foreach (var docTypeXml in docsXml)
            {
                ////create the model
                //BUG: We need to change this back once the Hive IO provider is fixed and it get its ProviderMetadata.MappingRoot  set
                // (APN) - I've changed back
                //var template = ((string)x.Attribute("template")).IsNullOrWhiteSpace() ? HiveId.Empty : new HiveId((Uri)null, "templates", new HiveIdValue((string)x.Attribute("template")));
                var templateId = ((string)docTypeXml.Attribute("template")).IsNullOrWhiteSpace() ? HiveId.Empty : new HiveId(new Uri("storage://templates"), "templates", new HiveIdValue((string)docTypeXml.Attribute("template")));

                //var docType = string.IsNullOrEmpty((string)x.Attribute("nodeType"))
                //                  ? null
                //                  : DocTypes.SingleOrDefault(d => d.Id == HiveId.ConvertIntToGuid((int)x.Attribute("nodeType")));

                var docType = _docTypes.SingleOrDefault(y => y.Alias.ToLowerInvariant() == docTypeXml.Name.LocalName.ToLowerInvariant());
                if (docType != null)
                {
                    // doctype exists so continue
                    var contentNode = CreateContentNodeFromXml(docType, templateId, docTypeXml);

                    //add the new model
                    _testRepository.Add(contentNode);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="templateId"> </param>
        /// <param name="x"></param>
        /// <returns></returns>
        private ContentEditorModel CreateContentNodeFromXml(DocumentTypeEditorModel docType, HiveId templateId, XElement x)
        {
            var parentId = x.Attribute("parentID");
            var contentNode = new ContentEditorModel(HiveId.ConvertIntToGuid((int)x.Attribute("id")))
            {
                Name = (string)x.Attribute("nodeName"),
                DocumentTypeId = docType == null ? HiveId.Empty : docType.Id,
                DocumentTypeName = docType == null ? "" : docType.Name,
                DocumentTypeAlias = docType == null ? "" : docType.Alias,
                ParentId = parentId != null ? HiveId.ConvertIntToGuid((int)parentId) : HiveId.Empty,

                //assign the properties
                Properties = GetNodeProperties(x, templateId)
            };
            return contentNode;
        }

        #endregion






        #region private GetNodeProperty

        public Guid GetNodeProperty(XElement node)
        {
            //we'll check if the property id map has been created first
            if (_nodePropertyIdMap == null)
                _nodePropertyIdMap = new List<KeyValuePair<KeyValuePair<int, string>, Guid>>();

            //check if the property map is found for the current model
            var id = (int)node.Parent.Attribute("id");
            var map = _nodePropertyIdMap.SingleOrDefault(x => x.Key.Key == id && x.Key.Value == node.Name.LocalName);
            if (map.Value == default(Guid))
            {
                //if there's no map, we create one, this should occur only once
                map = new KeyValuePair<KeyValuePair<int, string>, Guid>(
                    new KeyValuePair<int, string>(id, node.Name.LocalName),
                    Guid.NewGuid());
                _nodePropertyIdMap.Add(map);
            }
            return map.Value;
        }
        #endregion
    }
}
