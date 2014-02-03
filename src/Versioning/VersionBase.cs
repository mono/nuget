using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGet.Versioning
{
    public abstract class VersionBase : IVersion
    {
        public abstract string ToNormalizedString();

        public abstract int CompareTo(object obj);

        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        public override string ToString()
        {
            return ToNormalizedString();
        }

        public override int GetHashCode()
        {
            return ToNormalizedString().GetHashCode();
        }

        public static bool operator ==(VersionBase version1, VersionBase version2)
        {
            return CompareOperator(version1, version2) == 0;
        }

        public static bool operator !=(VersionBase version1, VersionBase version2)
        {
            return CompareOperator(version1, version2) != 0;
        }

        public static bool operator <(VersionBase version1, VersionBase version2)
        {
            return CompareOperator(version1, version2) < 0;
        }

        public static bool operator <=(VersionBase version1, VersionBase version2)
        {
            return CompareOperator(version1, version2) <= 0;
        }

        public static bool operator >(VersionBase version1, VersionBase version2)
        {
            return CompareOperator(version1, version2) > 0;
        }

        public static bool operator >=(VersionBase version1, VersionBase version2)
        {
            return CompareOperator(version1, version2) >= 0;
        }

        private static int CompareOperator(IVersion version1, IVersion version2)
        {
            if (Object.ReferenceEquals(version1, null))
                throw new ArgumentNullException("version1");

            return version1.CompareTo(version2);
        }
    }
}
