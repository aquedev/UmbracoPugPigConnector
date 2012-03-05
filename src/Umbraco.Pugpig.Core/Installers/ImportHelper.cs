using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Umbraco.Cms.Web;
using Umbraco.Cms.Web.Model;
using Umbraco.Cms.Web.Model.BackOffice.Editors;
using Umbraco.Cms.Web.Mvc;
using Umbraco.Framework;
using Umbraco.Framework.Context;
using Umbraco.Framework.Diagnostics;
using Umbraco.Framework.Persistence;
using Umbraco.Framework.Persistence.Model.Constants;
using Umbraco.Framework.Persistence.Model.Constants.AttributeDefinitions;
using Umbraco.Framework.Persistence.Model.Constants.AttributeTypes;

namespace Umbraco.Pugpig.Core.Installers
{
    public class UmbracoXmlImportHelper
    {
        #region properties

        // private const string FILE = "/App_Data/Schema.xml";
        private readonly XDocument _nodeData;
        private IEnumerable<DocumentTypeEditorModel> _docTypes;
        private IEnumerable<TemplateFile> _templates;
        private Dictionary<int, string> _creators;

        //defined internal attribute definitions/data types
        private List<DataType> _dataTypes;
        private DataType _selectedTemplateDataType;
        private DataType _nodeNameDataType;

        IAttributeTypeRegistry _attributeTypeRegistry;
        private IPropertyEditorFactory _propertyEditorFactory;

        private XDocument _Schema;


        #endregion



        #region Constructor

        public UmbracoXmlImportHelper(IAttributeTypeRegistry attributeTypeRegistry, IPropertyEditorFactory propertyEditorFactory)
        {
            _attributeTypeRegistry = attributeTypeRegistry;
            _propertyEditorFactory = propertyEditorFactory;
            _Schema = XDocument.Parse(Files.Schema);
        }

        #endregion



        #region InitDataTypes

        /// <summary>
        /// Creates the test data types
        /// </summary>
        public List<DataType> InitDataTypes(IFrameworkContext frameworkContext)
        {
            // much neater :)
            _dataTypes = CoreFakeCmsData.RequiredCoreUserAttributeTypes()
                            .Select(x => frameworkContext.TypeMappers.Map<DataType>(x))
                            .ToList();

            //_dataTypes = new List<DataType>();
            //foreach (var v in CoreCmsData.RequiredCoreUserAttributeTypes())
            //{
            //    var a = frameworkContext.TypeMappers.Map<DataType>(v);
            //    _dataTypes.Add(a);
            //}

            //get the data types from the CoreCmsData
            //var dataTypes = new List<DataType>()
            //        {
            //            frameworkContext.TypeMappers.Map<DataType>(CoreCmsData.RequiredCoreUserAttributeTypes().Single(x => x.Id == new HiveId("rte-pe".EncodeAsGuid()))),
            //            frameworkContext.TypeMappers.Map<DataType>(CoreCmsData.RequiredCoreUserAttributeTypes().Single(x => x.Id == new HiveId("sltb-pe".EncodeAsGuid()))),
            //            frameworkContext.TypeMappers.Map<DataType>(CoreCmsData.RequiredCoreUserAttributeTypes().Single(x => x.Id == new HiveId("csp-pe".EncodeAsGuid()))),
            //            frameworkContext.TypeMappers.Map<DataType>(CoreCmsData.RequiredCoreUserAttributeTypes().Single(x => x.Id == new HiveId("tag-pe".EncodeAsGuid()))),
            //            frameworkContext.TypeMappers.Map<DataType>(CoreCmsData.RequiredCoreUserAttributeTypes().Single(x => x.Id == new HiveId("mltb-pe".EncodeAsGuid()))),
            //            frameworkContext.TypeMappers.Map<DataType>(CoreCmsData.RequiredCoreUserAttributeTypes().Single(x => x.Id == new HiveId("media-picker-pe".EncodeAsGuid()))),
            //            frameworkContext.TypeMappers.Map<DataType>(CoreCmsData.RequiredCoreUserAttributeTypes().Single(x => x.Id == new HiveId("integer-pe".EncodeAsGuid()))),
            //            frameworkContext.TypeMappers.Map<DataType>(CoreCmsData.RequiredCoreUserAttributeTypes().Single(x => x.Id == new HiveId("uploader-pe".EncodeAsGuid())))
            //        };

            //_dataTypes = dataTypes;
            LogHelper.Error<UmbracoXmlImportHelper>(String.Format("There were {0} data types found", _dataTypes.Count), new Exception());
            return _dataTypes;
        }


        #endregion




        #region InitDocTypes

        public IEnumerable<DocumentTypeEditorModel> InitDocTypes()
        {
            try
            {
                // open xml file
                //var file = XDocument.Parse(File.ReadAllText(HttpContext.Current.Server.MapPath(FILE)));


                // create tabss
                Func<IEnumerable<Tab>> generateTabs = () => GetTabsTopLevel();

                var providerGroupRoot = new Uri("content://");

                var docTypes = CreateDocTypes(generateTabs, providerGroupRoot);

                // Set DocType Allowed Children
                SetUpAllowedChildren(docTypes, providerGroupRoot);

                // set up inheritence
                SetupDocTypeInheritence(docTypes, providerGroupRoot);

                return docTypes;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion





        #region InitTemplates
        /// <summary>
        /// Initialize templates from xml
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TemplateFile> InitTemplates()
        {
            var providerGroupRoot = new Uri("storage://templates");
            var templates = new List<TemplateFile>();

            //var file = XDocument.Parse(File.ReadAllText(HttpContext.Current.Server.MapPath(FILE)));

            var templatesXml = _Schema.Descendants("template");
            foreach (var xElement in templatesXml)
            {
                var filename = xElement.Attribute("Filename").Value;
                var name = xElement.Attribute("Name").Value;
                var tf = new TemplateFile(new HiveId(providerGroupRoot, "templates", new HiveIdValue(filename)))
                {
                    Name = name
                };
                templates.Add(tf);
            }

            _templates = templates;

            return templates;
        }

        #endregion




        #region private GetTabs

        /// <summary>
        /// Creates tabs from top level of xml
        /// </summary>
        /// <param name="tabsXml"></param>
        /// <returns></returns>
        private IEnumerable<Tab> GetTabsTopLevel()
        {
            var tabsXml = _Schema.Descendants("structure").Single().Elements("tabs").Single().Elements("tab");

            var tabs = new List<Tab>();
            foreach (var xElement in tabsXml)
            {
                var t = new Tab
                {
                    //Id = HiveId.ConvertIntToGuid(++_tabIdCounter),
                    Id = new HiveId(Guid.NewGuid()),
                    Alias = xElement.Attribute("Alias").Value,
                    Name = xElement.Attribute("Name").Value,
                    SortOrder = int.Parse(xElement.Attribute("SortOrder").Value)
                };
                tabs.Add(t);

            }

            //add the general tab
            var general = new Tab
            {
                //Id = HiveId.ConvertIntToGuid(++_tabIdCounter),
                Id = new HiveId(Guid.NewGuid()),
                Alias = FixedGroupDefinitions.GeneralGroup.Alias,
                Name = FixedGroupDefinitions.GeneralGroup.Name,
                SortOrder = FixedGroupDefinitions.GeneralGroup.Ordinal
            };
            tabs.Add(general);

            return tabs.ToArray();
        }

        #endregion



        #region private CreateDocTypes
        /// <summary>
        /// Create doctypes from xml
        /// </summary>
        /// <param name="generateTabs"></param>
        /// <param name="providerGroupRoot"></param>
        /// <returns></returns>
        private IEnumerable<DocumentTypeEditorModel> CreateDocTypes(Func<IEnumerable<Tab>> generateTabs, Uri providerGroupRoot)
        {
            var docTypes = new List<DocumentTypeEditorModel>();
            var docTypesContainerXml = _Schema.Descendants("documentTypes").Single();

            //var parentDocTypeId = FixedHiveIds.ContentRootSchema.ToString();
            var docTypesXml = docTypesContainerXml.Elements("documentType");

            foreach (var docTypeXml in docTypesXml)
            {
                // create the doctype
                var docType = CreateDocType(docTypeXml, generateTabs, providerGroupRoot);
                docTypes.Add(docType);
            }

            return docTypes;
        }

        #endregion



        #region private CreateDocType

        /// <summary>
        /// Creates doctype from xml
        /// </summary>
        /// <param name="docTypeXml"> </param>
        /// <param name="generateTabs"> </param>
        /// <param name="providerGroupRoot"> </param>
        /// <returns></returns>
        private DocumentTypeEditorModel CreateDocType(XElement docTypeXml, Func<IEnumerable<Tab>> generateTabs, Uri providerGroupRoot)
        {
            //var docType = new DocumentTypeEditorModel(HiveId.ConvertIntToGuid(id));
            var docType = new DocumentTypeEditorModel(new HiveId(Guid.NewGuid()));
            docType.Name = docTypeXml.Attribute("Name").Value;
            docType.Alias = docTypeXml.Attribute("Alias").Value;

            docType.Description = docTypeXml.Element("Description").Value;
            docType.Icon = docTypeXml.Element("Icon").Value;
            docType.Thumbnail = docTypeXml.Element("Thumbnail").Value;
            docType.AllowedTemplates = GetAllowedTemplates(docTypeXml);
            docType.DefaultTemplateId = docTypeXml.Element("DefaultTemplate").Value == string.Empty
                                            ? HiveId.Empty
                                            : _templates.Single(x => x.Name == docTypeXml.Element("DefaultTemplate").Value).Id;

            var tabs = generateTabs.Invoke().ToList();
            tabs.AddRange(GetTabs(docTypeXml));

            docType.DefinedTabs = new HashSet<Tab>(tabs);

            docType.IsAbstract = bool.Parse(docTypeXml.Element("IsAbstract").Value);

            // get properties
            docType.Properties = CreateProperties(docTypeXml, docType);

            return docType;
        }

        #endregion



        #region private  GetTabs
        /// <summary>
        /// Gets tabs from xml
        /// </summary>
        /// <param name="docTypeXml"></param>
        /// <returns></returns>
        private HashSet<Tab> GetTabs(XElement docTypeXml)
        {
            var tabsXml = docTypeXml.Descendants("tab");

            var tabs = new HashSet<Tab>();
            foreach (var xElement in tabsXml)
            {
                var alias = xElement.Attribute("Alias").Value;
                var name = xElement.Attribute("Name").Value;
                var sortOrder = xElement.Attribute("SortOrder").Value;
                var tab = new Tab()
                {
                    Alias = alias,
                    Name = name,
                    SortOrder = int.Parse(sortOrder)
                };
                tabs.Add(tab);
            }
            return tabs;
        }

        #endregion



        #region private  GetInheritedTabs

        /// <summary>
        /// Gets tabs and inherited tabs from parent doctype.
        /// </summary>
        /// <param name="inheritedFromDocType"></param>
        /// <returns></returns>
        private HashSet<Tab> GetInheritedTabs(DocumentTypeEditorModel inheritedFromDocType)
        {
            var inheritedTabs = new HashSet<Tab>(inheritedFromDocType.DefinedTabs);
            //foreach (var t in inheritedFromDocType.InheritedTabs)
            //{
            //    inheritedTabs.Add(t);
            //}
            return inheritedTabs;
        }

        #endregion



        #region private  GetInheritedPropertiess

        /// <summary>
        /// Gets properties and inherited properties from parent doctype.
        /// </summary>
        /// <param name="inheritedFromDocType"></param>
        /// <returns></returns>
        private HashSet<DocumentTypeProperty> GetInheritedProperties(DocumentTypeEditorModel inheritedFromDocType)
        {
            var inheritedProps = new HashSet<DocumentTypeProperty>(inheritedFromDocType.Properties);
            //foreach (var t in inheritedFromDocType.InheritedProperties)
            //{
            //    inheritedProps.Add(t);
            //}
            return inheritedProps;
        }

        #endregion



        #region private GetProperties
        /// <summary>
        /// Create properties from xml
        /// </summary>
        /// <param name="docTypeXml"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        private new HashSet<DocumentTypeProperty> CreateProperties(XElement docTypeXml, DocumentTypeEditorModel docType)
        {
            var properties = new HashSet<DocumentTypeProperty>();

            properties.Add(CreateNodeName(docType));
            properties.Add(CreateSelectedTemplate(docType));

            var propsXml = docTypeXml.Descendants("property");

            foreach (var xElement in propsXml)
            {
                var name = xElement.Attribute("Name").Value;
                var alias = xElement.Attribute("Alias").Value;

                var dataTypeAlias = xElement.Element("DataTypeAlias").Value;
                var sortOrder = int.Parse(xElement.Attribute("SortOrder").Value);
                var tabAlias = xElement.Attribute("TabAlias").Value;

                LogHelper.Error<UmbracoXmlImportHelper>(String.Format("the data type alias was {0}", dataTypeAlias), new Exception());
                var dataType = _dataTypes.First(x => x.Alias == dataTypeAlias);

                // TODO: property is not in tab...possibly need object initializer?

                var p = new DocumentTypeProperty(new HiveId(Guid.NewGuid()), dataType)
                {
                    Name = name,
                    Alias = alias,
                   // TabId = docType.DefinedTabs.Single(x => x.Alias == tabAlias).Id,
                    SortOrder = sortOrder
                };
                properties.Add(p);
            }

            return properties;
        }

        #endregion



        #region private CreateNodeName
        /// <summary>
        /// Creates Node Name property
        /// </summary>
        /// <param name="docType"></param>
        /// <returns></returns>
        private DocumentTypeProperty CreateNodeName(DocumentTypeEditorModel docType)
        {
            var nodeNameAttributeType = _attributeTypeRegistry.GetAttributeType(NodeNameAttributeType.AliasValue);
            _nodeNameDataType = new DataType(
                nodeNameAttributeType.Id,
                nodeNameAttributeType.Name,
                nodeNameAttributeType.Alias,
                _propertyEditorFactory.GetPropertyEditor(new Guid(CorePluginConstants.NodeNamePropertyEditorId)).Value,
                string.Empty);


            var dtp = new DocumentTypeProperty(new HiveId(Guid.NewGuid()), _nodeNameDataType);
            dtp.Name = "Node Name";
            dtp.Alias = NodeNameAttributeDefinition.AliasValue;
            dtp.TabId = docType.DefinedTabs.Single(x => x.Alias == FixedGroupDefinitions.GeneralGroup.Alias).Id;
            dtp.SortOrder = 0;

            return dtp;
        }

        #endregion



        #region private CreateSelectedTemplate
        /// <summary>
        /// Creates selected template property
        /// </summary>
        /// <param name="docType"></param>
        /// <returns></returns>
        private DocumentTypeProperty CreateSelectedTemplate(DocumentTypeEditorModel docType)
        {

            var selectedTemplateAttributeType = _attributeTypeRegistry.GetAttributeType(SelectedTemplateAttributeType.AliasValue);
            _selectedTemplateDataType = new DataType(
                selectedTemplateAttributeType.Id,
                selectedTemplateAttributeType.Name,
                selectedTemplateAttributeType.Alias,
                _propertyEditorFactory.GetPropertyEditor(new Guid(CorePluginConstants.SelectedTemplatePropertyEditorId)).Value,
                string.Empty);


            var dtp = new DocumentTypeProperty(new HiveId(Guid.NewGuid()), _selectedTemplateDataType);

            dtp.Name = "Selected template";
            dtp.Alias = SelectedTemplateAttributeDefinition.AliasValue;
            //dtp.TabId = docType.DefinedTabs.Where(x => x.Alias == _generalGroup.Alias).Single().Id;
            dtp.TabId = docType.DefinedTabs.Single(x => x.Alias == FixedGroupDefinitions.GeneralGroup.Alias).Id;
            dtp.SortOrder = 0;
            return dtp;
        }

        #endregion



        #region private GetAllowedTemplates
        /// <summary>
        /// 
        /// </summary>
        /// <param name="docTypeXml"></param>
        /// <returns></returns>
        private IEnumerable<SelectListItem> GetAllowedTemplates(XElement docTypeXml)
        {
            var templateSelectList = new List<SelectListItem>();
            var allowed = docTypeXml.Element("AllowedTemplates").Value.Split(",".ToCharArray(),
                                                                               StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in allowed)
            {
                var temp = _templates.Single(x => x.Name == s);

                var item = new SelectListItem
                {
                    Selected = true,
                    Text = temp.Name,
                    Value = temp.Id.ToString()
                };
                templateSelectList.Add(item);
            }

            return templateSelectList;
        }

        #endregion



        #region private SetupDocTypeInheritence

        /// <summary>
        /// TODO: there is something strange going on where inherited doctypes appear multiple times in the gui
        /// </summary>
        /// <param name="docTypes"> </param>
        /// <param name="xml"></param>
        /// <param name="providerGroupRoot"></param>
        public IEnumerable<DocumentTypeEditorModel> SetupDocTypeInheritence(IEnumerable<DocumentTypeEditorModel> docTypes, Uri providerGroupRoot)
        {
            // get all doctypes from xml
            var docTypesXml = _Schema.Descendants("documentType");

            // iterate over doctypes xml so we can wire up allowed children
            foreach (var xElement in docTypesXml)
            {
                var inheritedFrom = new List<HierarchicalSelectListItem>()
                                        {
                                            new HierarchicalSelectListItem()
                                                {
                                                    Selected = true,
                                                    Value = FixedHiveIds.ContentRootSchema.ToString()
                                                }
                                        };

                // get doctype from list
                var docType = docTypes.Single(x => x.Alias == xElement.Attribute("Alias").Value);

                // get InheritedFrom aliases from csv
                var aliases = xElement.Element("InheritedFrom").Value.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                //var parentDocTypeId = FixedHiveIds.ContentRootSchema.ToString();

                foreach (var alias in aliases)
                {
                    // get InheritedFrom doctype from list
                    var inheritedFromDocType = docTypes.Single(x => x.Alias == alias);

                    // add parent doctype
                    inheritedFrom.Add(new HierarchicalSelectListItem() { Selected = true, Value = inheritedFromDocType.Id.ToString() });

                    // these are not required
                    // get inherited tabs
                    //  docType.InheritedTabs = GetInheritedTabs(inheritedFromDocType);

                    // get inherited props
                    //  docType.InheritedProperties = GetInheritedPropertiess(inheritedFromDocType);
                }

                // get doctype parents
                docType.InheritFrom = inheritedFrom;
            }

            return docTypes;
        }

        #endregion



        #region private SetUpAllowedChildren

        /// <summary>
        /// Gets allowed children form xml.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="providerGroupRoot"></param>
        /// <returns></returns>
        private void SetUpAllowedChildren(IEnumerable<DocumentTypeEditorModel> docTypes, Uri providerGroupRoot)
        {
            // get all doctypes from xml
            var docTypesXml = _Schema.Descendants("documentType");


            // iterate over doctypes xml so we can wire up allowed children
            foreach (var xElement in docTypesXml)
            {
                // get all doctyes which are not abstract
                var docTypeSelectList = docTypes
                                            .Where(x => !x.IsAbstract)
                                            .Select(x => new SelectListItem()
                                            {
                                                Selected = false,

                                                Value = new HiveId(providerGroupRoot, "nhibernate", new HiveIdValue(new Guid(x.Id.Value.ToString()))).ToString()
                                                //Value = x.Id.ToString()
                                            }).ToList();

                // get doctype from list
                var docType = docTypes.Single(x => x.Alias == xElement.Attribute("Alias").Value);

                // get allowed children aliases from csv
                var aliases = xElement.Element("AllowedChildren").Value.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                var allowedChildren = new List<SelectListItem>();
                // iterate over selected aliases, set Selected = true in allowedChildren
                foreach (var alias in aliases)
                {
                    // get child doctype from list
                    var allowedChildDocType = docTypes.Single(x => x.Alias == alias);
                    //var item = docTypeSelectList.Single(x => x.Value == new HiveId(providerGroupRoot, "nhibernate", new HiveIdValue(new Guid(allowedChildDocType.Id.Value.ToString()))).ToString());
                    //item.Selected = true;

                    docTypeSelectList.Single(
                        x =>
                        x.Value ==
                        new HiveId(providerGroupRoot, "nhibernate",
                                   new HiveIdValue(new Guid(allowedChildDocType.Id.Value.ToString()))).ToString()).
                        Selected = true;
                }

                // save!
                docType.AllowedChildren = docTypeSelectList;
            }

        }

        #endregion
    }
}
