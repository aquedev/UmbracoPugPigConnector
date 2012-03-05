// Type: Umbraco.Framework.Persistence.Model.Versioning.RevisionStatusType
// Assembly: Umbraco.Framework.Persistence, Version=5.0.0.31827, Culture=neutral
// Assembly location: C:\Projects\UmbracoPupPigConnector\lib\Umbraco\Umbraco.Framework.Persistence.dll

using Umbraco.Framework;

namespace Umbraco.Framework.Persistence.Model.Versioning
{
    public class RevisionStatusType : IReferenceByName, IReferenceByAlias
    {
        public RevisionStatusType();
        public RevisionStatusType(HiveId systemId, string alias, LocalizedString name, bool isSystem);
        public RevisionStatusType(string alias, string name);
        public virtual HiveId Id { get; set; }
        public virtual bool IsSystem { get; set; }

        #region IReferenceByName Members

        public virtual string Alias { get; set; }
        public virtual LocalizedString Name { get; set; }

        #endregion

        public string GetNameOrAlias();
    }
}
