// Type: Umbraco.Framework.DemandsDependenciesAttribute
// Assembly: Umbraco.Framework, Version=5.0.0.31826, Culture=neutral
// Assembly location: C:\Projects\UmbracoPupPigConnector\lib\Umbraco\Umbraco.Framework.dll

using System;

namespace Umbraco.Framework
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DemandsDependenciesAttribute : Attribute
    {
        public DemandsDependenciesAttribute(Type demandBuilderType);
        public Type DemandBuilderType { get; protected set; }
    }
}
