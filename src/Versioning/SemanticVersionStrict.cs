using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NuGet.Versioning
{
    public class SemanticVersionStrict : ISemanticVersion
    {
        private const RegexOptions _flags = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
        private static readonly Regex _strictSemanticVersionRegex = new Regex(@"^(?<Version>\d+(\.\d+){2})(?<Release>-[0-9A-Za-z-]*(\.[0-9A-Za-z-])*)?(?<Metadata>\+[0-9A-Za-z-]*)?$", _flags);
        private readonly int _major;
        private readonly int _minor;
        private readonly int _patch;
        private readonly IEnumerable<string> _prereleaseLabels;
        private readonly string _metadata;

        protected SemanticVersionStrict(Version version, string prereleaseLabel, string metadata)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            _major = version.Major < 0 ? 0 : version.Major;
            _minor = version.Minor < 0 ? 0 : version.Minor;
            _patch = version.Build < 0 ? 0 : version.Build;
            _metadata = metadata;

            if (!String.IsNullOrEmpty(prereleaseLabel))
                _prereleaseLabels = prereleaseLabel.Split('.');
        }

        protected SemanticVersionStrict(SemanticVersionStrict strictVersion)
            : this(strictVersion.Major, strictVersion.Minor, strictVersion.Patch, strictVersion.ReleaseLabels, strictVersion.Metadata)
        { }

        public SemanticVersionStrict(int major, int minor, int patch)
            : this(major, minor, patch, null)
        { }

        public SemanticVersionStrict(int major, int minor, int patch, string prereleaseLabel)
            : this(major, minor, patch, prereleaseLabel, null)
        { }

        public SemanticVersionStrict(int major, int minor, int patch, string prereleaseLabel, string metadata)
            : this(major, minor, patch, prereleaseLabel == null ? null : prereleaseLabel.Split('.'), metadata)
        { }

        public SemanticVersionStrict(int major, int minor, int patch, IEnumerable<string> prereleaseLabels, string metadata)
        {
            _major = major;
            _minor = minor;
            _patch = patch;
            _prereleaseLabels = prereleaseLabels;
            _metadata = metadata;
        }

        public int Major { get { return _major; } }
        public int Minor { get { return _minor; } }
        public int Patch { get { return _patch; } }


        public IEnumerable<string> ReleaseLabels
        {
            get
            {
                return _prereleaseLabels;
            }
        }

        /// <summary>
        /// Gets the optional special version.
        /// </summary>
        public string SpecialVersion
        {
            get
            {
                if (ReleaseLabels != null)
                {
                    return String.Join(".", _prereleaseLabels);
                }

                return string.Empty;
            }
        }

        public virtual bool IsPrerelease
        {
            get
            {
                if (ReleaseLabels != null)
                {
                    var enumerator = ReleaseLabels.GetEnumerator();
                    return (enumerator.MoveNext() && !String.IsNullOrEmpty(enumerator.Current));
                }

                return false;
            }
        }

        public virtual bool HasMetadata
        {
            get
            {
                return !String.IsNullOrEmpty(Metadata);
            }
        }

        public virtual string Metadata
        {
            get
            {
                return _metadata;
            }
        }

        public string ToNormalizedString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}.{1}.{2}", Major, Minor, Patch);

            if (IsPrerelease)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "-{0}", SpecialVersion);
            }

            if (HasMetadata)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "+{0}", Metadata);
            }

            return sb.ToString();
        }

        public static bool TryParse(string version, out SemanticVersionStrict value)
        {
            value = null;

            if (String.IsNullOrEmpty(version))
            {
                return false;
            }

            var match = _strictSemanticVersionRegex.Match(version.Trim());

            Version versionValue;
            if (!match.Success || !Version.TryParse(match.Groups["Version"].Value, out versionValue))
            {
                return false;
            }

            Version ver = NormalizeVersionValue(versionValue);

            value = new SemanticVersionStrict(ver.Major, ver.Minor, ver.Build, match.Groups["Release"].Value.TrimStart('-'), match.Groups["Metadata"].Value.TrimStart('+'));

            return true;
        }

        #region Compare

        public override int GetHashCode()
        {
            return ToNormalizedString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ISemanticVersion);
        }

        public virtual bool Equals(ISemanticVersion other)
        {
            return CompareTo(other) == 0;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as ISemanticVersion);
        }

        public virtual int CompareTo(ISemanticVersion other)
        {
            return Compare(this, other);
        }

        public static bool operator ==(SemanticVersionStrict version1, SemanticVersionStrict version2)
        {
            return Compare(version1, version2) == 0;
        }

        public static bool operator !=(SemanticVersionStrict version1, SemanticVersionStrict version2)
        {
            return Compare(version1, version2) != 0;
        }

        public static bool operator <(SemanticVersionStrict version1, SemanticVersionStrict version2)
        {
            return Compare(version1, version2) < 0;
        }

        public static bool operator <=(SemanticVersionStrict version1, SemanticVersionStrict version2)
        {
            return Compare(version1, version2) <= 0;
        }

        public static bool operator >(SemanticVersionStrict version1, SemanticVersionStrict version2)
        {
            return Compare(version1, version2) > 0;
        }

        public static bool operator >=(SemanticVersionStrict version1, SemanticVersionStrict version2)
        {
            return Compare(version1, version2) >= 0;
        }

        #endregion

        #region Helper Methods

        protected static Version NormalizeVersionValue(Version version)
        {
            return new Version(version.Major,
                               version.Minor,
                               Math.Max(version.Build, 0),
                               Math.Max(version.Revision, 0));
        }

        private static int Compare(ISemanticVersion version1, ISemanticVersion version2)
        {
            return VersionComparer.Strict.Compare(version1, version2);
        }

        #endregion
    }
}
