using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

namespace NuGet.Versioning
{
    public class VersionSpec : IVersionSpec
    {
        private readonly ISemanticVersion _minVersion;
        private readonly bool _isMinInclusive;
        private readonly ISemanticVersion _maxVersion;
        private readonly bool _isMaxInclusive;
        private readonly IVersionComparer _comparer;

        /// <summary>
        /// A VersionSpec that matches only the given version.
        /// </summary>
        /// <param name="version">The minimum and maximum version requirement.</param>
        public VersionSpec(ISemanticVersion version)
            : this(minVersion: version, isMinInclusive: true, maxVersion: version, isMaxInclusive: true)
        {

        }

        /// <summary>
        /// A VersionSpec with no max version.
        /// </summary>
        /// <param name="minVersion">The minimum version requirement.</param>
        /// <param name="minInclusive">True if minVersion meets the requirement.</param>
        public VersionSpec(ISemanticVersion minVersion, bool isMinInclusive)
            : this(minVersion: minVersion, isMinInclusive: isMinInclusive, maxVersion: null, isMaxInclusive: false)
        {

        }

        public VersionSpec(ISemanticVersion minVersion=null, ISemanticVersion maxVersion=null,
            bool isMinInclusive=false, bool isMaxInclusive=false, IVersionComparer comparer=null)
        {
            _minVersion = minVersion;
            _maxVersion = maxVersion;
            _isMinInclusive = isMinInclusive;
            _isMaxInclusive = IsMaxInclusive;
            _comparer = comparer ?? new VersionComparer();
        }

        public ISemanticVersion MinVersion
        {
            get
            {
                return _minVersion;
            }
        }

        public ISemanticVersion MaxVersion
        {
            get
            {
                return _maxVersion;
            }
        }

        public bool IsMaxInclusive
        {
            get
            {
                return _isMaxInclusive;
            }
        }

        public bool IsMinInclusive
        {
            get
            {
                return _isMinInclusive;
            }
        }

        /// <summary>
        /// Determines if the specified version is within the version spec
        /// </summary>
        public bool Satisfies(ISemanticVersion version)
        {
            return Satisfies(version, _comparer);
        }

        public bool Satisfies(ISemanticVersion version, VersionComparison mode)
        {
            return Satisfies(version, new VersionComparer(mode));
        }

        public bool Satisfies(ISemanticVersion version, IVersionComparer comparer)
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

        /// <summary>
        /// The version string is either a simple version or an arithmetic range
        /// e.g.
        ///      1.0         --> 1.0 ≤ x
        ///      (,1.0]      --> x ≤ 1.0
        ///      (,1.0)      --> x &lt; 1.0
        ///      [1.0]       --> x == 1.0
        ///      (1.0,)      --> 1.0 &lt; x
        ///      (1.0, 2.0)   --> 1.0 &lt; x &lt; 2.0
        ///      [1.0, 2.0]   --> 1.0 ≤ x ≤ 2.0
        /// </summary>
        public static IVersionSpec ParseVersionSpec(string value)
        {
            IVersionSpec versionInfo;
            if (!VersionSpec.TryParseVersionSpec(value, out versionInfo))
            {
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentCulture,
                     Resources.InvalidVersionString, value));
            }

            return versionInfo;
        }

        public static bool TryParseVersionSpec(string value, out IVersionSpec result)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var versionSpec = new VersionSpec();
            value = value.Trim();

            // First, try to parse it as a plain version string
            NuGetVersion version;
            if (NuGetVersion.TryParse(value, out version))
            {
                // A plain version is treated as an inclusive minimum range
                result = new VersionSpec(version, true);

                return true;
            }

            // It's not a plain version, so it must be using the bracket arithmetic range syntax

            result = null;

            // Fail early if the string is too short to be valid
            if (value.Length < 3)
            {
                return false;
            }

            bool isMinInclusive, isMaxInclusive;
            ISemanticVersion minVersion = null, maxVersion = null;

            // The first character must be [ to (
            switch (value.Substring(0, 1))
            {
                case "[":
                    isMinInclusive = true;
                    break;
                case "(":
                    isMinInclusive = false;
                    break;
                default:
                    return false;
            }

            // The last character must be ] ot )
            switch (value.Substring(value.Length-1, 1))
            {
                case "]":
                    isMaxInclusive = true;
                    break;
                case ")":
                    isMaxInclusive = false;
                    break;
                default:
                    return false;
            }

            // Get rid of the two brackets
            value = value.Substring(1, value.Length - 2);

            // Split by comma, and make sure we don't get more than two pieces
            string[] parts = value.Split(',');
            if (parts.Length > 2)
            {
                return false;
            }
            else if (parts.All(String.IsNullOrEmpty))
            {
                // If all parts are empty, then neither of upper or lower bounds were specified. Version spec is of the format (,]
                return false;
            }

            // If there is only one piece, we use it for both min and max
            string minVersionString = parts[0];
            string maxVersionString = (parts.Length == 2) ? parts[1] : parts[0];

            // Only parse the min version if it's non-empty
            if (!String.IsNullOrWhiteSpace(minVersionString))
            {
                if (!TryParseVersion(minVersionString, out version))
                {
                    return false;
                }
                minVersion = version;
            }

            // Same deal for max
            if (!String.IsNullOrWhiteSpace(maxVersionString))
            {
                if (!TryParseVersion(maxVersionString, out version))
                {
                    return false;
                }
                maxVersion = version;
            }

            // Successful parse!
            result = new VersionSpec(minVersion, maxVersion, isMinInclusive, isMaxInclusive);
            return true;
        }

        public override string ToString()
        {
            if (MinVersion != null && IsMinInclusive && MaxVersion == null && !IsMaxInclusive)
            {
                return MinVersion.ToString();
            }

            if (MinVersion != null && MaxVersion != null && MinVersion == MaxVersion && IsMinInclusive && IsMaxInclusive)
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

        private static bool TryParseVersion(string versionString, out NuGetVersion version)
        {
            version = null;
            if (!NuGetVersion.TryParse(versionString, out version))
            {
                // Support integer version numbers (i.e. 1 -> 1.0)
                int versionNumber;
                if (Int32.TryParse(versionString, out versionNumber) && versionNumber > 0)
                {
                    version = new NuGetVersion(new Version(versionNumber, 0));
                }
            }
            return version != null;
        }
    }
}