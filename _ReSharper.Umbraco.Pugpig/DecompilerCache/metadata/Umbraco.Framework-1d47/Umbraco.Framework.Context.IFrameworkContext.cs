// Type: Umbraco.Framework.Context.IFrameworkContext
// Assembly: Umbraco.Framework, Version=5.0.0.31826, Culture=neutral
// Assembly location: C:\Projects\UmbracoPupPigConnector\lib\Umbraco\Umbraco.Framework.dll

using System;
using Umbraco.Framework;
using Umbraco.Framework.Localization;
using Umbraco.Framework.Tasks;
using Umbraco.Framework.TypeMapping;

namespace Umbraco.Framework.Context
{
    public interface IFrameworkContext : IDisposable
    {
        ApplicationTaskManager TaskManager { get; }
        LanguageInfo CurrentLanguage { get; set; }
        TextManager TextManager { get; }
        MappingEngineCollection TypeMappers { get; }
        AbstractFinalizer ScopedFinalizer { get; }
        AbstractScopedCache ScopedCache { get; }
        AbstractApplicationCache ApplicationCache { get; }
    }
}
