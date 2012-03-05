// Type: Umbraco.Hive.Tasks.HiveProviderInstallTask
// Assembly: Umbraco.Hive, Version=5.0.0.31827, Culture=neutral
// Assembly location: C:\Projects\UmbracoPupPigConnector\lib\Umbraco\Umbraco.Hive.dll

using Umbraco.Framework.Context;
using Umbraco.Framework.Tasks;
using Umbraco.Hive;

namespace Umbraco.Hive.Tasks
{
    public abstract class HiveProviderInstallTask : ProviderInstallTask
    {
        protected HiveProviderInstallTask(IFrameworkContext context, IHiveManager coreManager);
        public IHiveManager CoreManager { get; }
    }
}
