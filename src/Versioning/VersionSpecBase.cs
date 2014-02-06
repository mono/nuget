using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NuGet.Versioning
{
    public abstract class VersionSpecBase<T> where T:IVersion, IVersionSpec<T>
    {
        public VersionSpecBase()
        {

        }

        public VersionSpecBase(T version)
        {
            IsMinInclusive = true;
            IsMaxInclusive = true;
            MinVersion = version;
            MaxVersion = version;
        }

        public T MinVersion { get; set; }
        public bool IsMinInclusive { get; set; }
        public T MaxVersion { get; set; }
        public bool IsMaxInclusive { get; set; }

        /// <summary>
        /// Determines if the specified version is within the version spec
        /// </summary>
        public bool Satisfies(T version)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            bool condition = true;
            if (MinVersion != null)
            {
                if (IsMinInclusive)
                {
                    condition &= version.CompareTo(MinVersion) >= 0;
                }
                else
                {
                    condition &= version.CompareTo(MinVersion) > 0;
                }
            }

            if (MaxVersion != null)
            {
                if (IsMaxInclusive)
                {
                    condition &= version.CompareTo(MaxVersion) <= 0;
                }
                else
                {
                    condition &= version.CompareTo(MaxVersion) < 0;
                }
            }

            return condition;
        }


        public override string ToString()
        {
            if (MinVersion != null && IsMinInclusive && MaxVersion == null && !IsMaxInclusive)
            {
                return MinVersion.ToString();
            }

            if (MinVersion != null && MaxVersion != null && MinVersion.Equals(MaxVersion) && IsMinInclusive && IsMaxInclusive)
            {
                return "[" + MinVersion + "]";
            }

            var versionBuilder = new StringBuilder();
            versionBuilder.Append(IsMinInclusive ? '[' : '(');
            versionBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}, {1}", MinVersion, MaxVersion);
            versionBuilder.Append(IsMaxInclusive ? ']' : ')');

            return versionBuilder.ToString();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            // TODO: Implement format provider
            return ToString();
        }
    }
}
