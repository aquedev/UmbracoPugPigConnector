// Type: Umbraco.Framework.HiveId
// Assembly: Umbraco.Framework, Version=5.0.0.31826, Culture=neutral
// Assembly location: C:\Projects\UmbracoPupPigConnector\lib\Umbraco\Umbraco.Framework.dll

using System;
using System.ComponentModel;

namespace Umbraco.Framework
{
    [TypeConverter(typeof (HiveIdTypeConverter))]
    public struct HiveId : IEquatable<HiveId>
    {
        public static readonly HiveId Empty;
        public HiveId(string value);
        public HiveId(int value);
        public HiveId(Uri value);
        public HiveId(Guid value);
        public HiveId(HiveIdValue value);
        public HiveId(string providerGroupSchemeOnly, string providerId, HiveIdValue value);
        public HiveId(Uri providerGroupRoot, string providerId, HiveIdValue value);
        public Uri ProviderGroupRoot { get; private set; }
        public string ProviderId { get; private set; }
        public HiveIdValue Value { get; private set; }

        #region IEquatable<HiveId> Members

        public bool Equals(HiveId other);

        #endregion

        public static bool operator ==(HiveId left, HiveId right);
        public static bool operator !=(HiveId left, HiveId right);
        public static explicit operator HiveId(Guid value);
        public static explicit operator HiveId(int value);
        public bool IsSystem();
        public Uri ToUri();
        public static HiveId ConvertIntToGuid(int value);
        public static HiveId ConvertIntToGuid(string providerGroupSchemeOnly, string providerId, int value);
        public static HiveId ConvertIntToGuid(Uri providerGroupRoot, string providerId, int value);
        public override bool Equals(object obj);
        public override int GetHashCode();
        public bool EqualsIgnoringProviderId(HiveId other);
        public string ToFriendlyString();
        public override string ToString();
        public string ToString(HiveIdFormatStyle style);
        public static HiveId Parse(string formattedValue);
        public static AttemptTuple<HiveId> TryParse(string formattedValue);
    }
}
