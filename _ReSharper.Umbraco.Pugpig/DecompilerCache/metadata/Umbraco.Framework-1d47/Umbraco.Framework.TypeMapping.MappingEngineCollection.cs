// Type: Umbraco.Framework.TypeMapping.MappingEngineCollection
// Assembly: Umbraco.Framework, Version=5.0.0.31826, Culture=neutral
// Assembly location: C:\Projects\UmbracoPupPigConnector\lib\Umbraco\Umbraco.Framework.dll

using System;
using System.Collections.Generic;

namespace Umbraco.Framework.TypeMapping
{
    public class MappingEngineCollection : AbstractMappingEngine
    {
        public MappingEngineCollection(IEnumerable<Lazy<AbstractMappingEngine, TypeMapperMetadata>> binders);
        public IEnumerable<Lazy<AbstractMappingEngine, TypeMapperMetadata>> Binders { get; }
        public void Add(Lazy<AbstractMappingEngine, TypeMapperMetadata> binder);

        public AbstractMappingEngine GetMapHandler(Type sourceType, Type destinationType,
                                                   bool allowInheritedTypes = true);

        public override object Map(object source, Type sourceType, Type destinationType);
        public override void Map(object source, object destination, Type sourceType, Type destinationType);
        public override void Configure();
        public override IEnumerable<TypeMapperMetadata> GetDynamicSupportedMappings();
        public override bool TryGetDestinationType(Type sourceType, out Type destinationType);
        public override bool TryGetDestinationType(Type sourceType, Type baseDestinationType, out Type destinationType);
    }
}
