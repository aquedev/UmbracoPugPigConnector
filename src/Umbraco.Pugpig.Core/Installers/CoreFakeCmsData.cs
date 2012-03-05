using System.Collections.Generic;
using Umbraco.Framework.Persistence;
using Umbraco.Framework.Persistence.Model.Attribution.MetaData;

namespace Umbraco.Pugpig.Core.Installers
{
    public class CoreFakeCmsData
    {
        internal static IEnumerable<AttributeType> RequiredCoreUserAttributeTypes()
        {
            return new[]
                {
                    AttributeTypeRegistry.Current.GetAttributeType("richTextEditor"),
                    AttributeTypeRegistry.Current.GetAttributeType("singleLineTextBox"),
                    AttributeTypeRegistry.Current.GetAttributeType("multiLineTextBox"),
                    AttributeTypeRegistry.Current.GetAttributeType("multiLineTextBoxWithControls"),
                    AttributeTypeRegistry.Current.GetAttributeType("colorSwatchPicker"),
                    AttributeTypeRegistry.Current.GetAttributeType("tags"),
                    AttributeTypeRegistry.Current.GetAttributeType("contentPicker"),
                    AttributeTypeRegistry.Current.GetAttributeType("mediaPicker"),
                    AttributeTypeRegistry.Current.GetAttributeType("integer"),
                    AttributeTypeRegistry.Current.GetAttributeType("decimal"),
                    AttributeTypeRegistry.Current.GetAttributeType("uploader"),
                    AttributeTypeRegistry.Current.GetAttributeType("trueFalse"),
                    AttributeTypeRegistry.Current.GetAttributeType("dropdownList"),
                    AttributeTypeRegistry.Current.GetAttributeType("listBox"),
                    AttributeTypeRegistry.Current.GetAttributeType("checkboxList"),
                    AttributeTypeRegistry.Current.GetAttributeType("radioButtonList"),
                    AttributeTypeRegistry.Current.GetAttributeType("dateTimePicker"),
                    AttributeTypeRegistry.Current.GetAttributeType("slider"),
                    AttributeTypeRegistry.Current.GetAttributeType("label")
                };
        }
    }
}