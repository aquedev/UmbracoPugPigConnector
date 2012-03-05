// Type: Umbraco.Framework.TypeMapping.AbstractMappingEngine
// Assembly: Umbraco.Framework, Version=5.0.0.31826, Culture=neutral
// Assembly location: C:\Projects\UmbracoPupPigConnector\lib\Umbraco\Umbraco.Framework.dll

using System;
using System.Collections.Generic;

namespace Umbraco.Framework.TypeMapping
{
    public abstract class AbstractMappingEngine
    {
        public virtual bool IsConfigured { get; protected set; }
        public virtual TDestination Map<TSource, TDestination>(TSource source);
        public TDestination Map<TDestination>(object source);

        public virtual void Map<TSource, TDestination>(TSource source, TDestination destination)
            where TDestination : class;

        public abstract object Map(object source, Type sourceType, Type destinationType);
        public abstract void Map(object source, object destination, Type sourceType, Type destinationType);
        public abstract void Configure();
        public abstract IEnumerable<TypeMapperMetadata> GetDynamicSupportedMappings();
        public TDestination Map<TDestination>(object source, Action<TDestination> afterMapIfNotNull);
        public void WeakMap(object source, object destination);
        public abstract bool TryGetDestinationType(Type sourceType, out Type destinationType);
        public abstract bool TryGetDestinationType(Type sourceType, Type baseDestinationType, out Type destinationType);
        public T MapToIntent<T>(object source) where T : class;
        public object MapToIntent(object source, Type baseDestinationType);
        public virtual bool TryGetDestinationType<TSource>(out Type destinationType);
        public virtual bool TryGetDestinationType<TSource, TBaseOfDestination>(out Type destinationType);
    }
}
