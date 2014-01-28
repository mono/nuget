using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace NuGet.Versioning
{
    /// <summary>
    /// A hybrid implementation of SemVer that supports semantic versioning as described at http://semver.org while not strictly enforcing it to 
    /// allow older 4-digit versioning schemes to continue working.
    /// </summary>
    public sealed class SemanticVersion : SemanticVersionStrict, ISemanticVersion
    {
        private const RegexOptions _flags = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
        private static readonly Regex _semanticVersionRegex = new Regex(@"^(?<Version>\d+(\s*\.\s*\d+){0,3})(?<Release>-[a-z][0-9a-z-]*)?$", _flags);
        private readonly string _originalString;
        private Version _version;

        public SemanticVersion(string version)
            : this(Parse(version))
        {
            // The constructor normalizes the version string so that it we do not need to normalize it every time we need to operate on it. 
            // The original string represents the original form in which the version is represented to be used when printing.
            _originalString = version;
        }

        public SemanticVersion(SemanticVersionStrict strict)
            : base(strict)
        {

        }


        public SemanticVersion(int major, int minor, int patch, int revision)
            : this(new Version(major, minor, patch, revision))
        {

        }

        public SemanticVersion(int major, int minor, int patch, string specialVersion)
            : this(new Version(major, minor, patch), specialVersion)
        {
        }

        public SemanticVersion(Version version)
            : this(version, String.Empty)
        {

        }

        public SemanticVersion(Version version, string specialVersion)
            : this(version, specialVersion, null)
        {

        }

        private SemanticVersion(Version version, string specialVersion, string originalString)
            : base(version, specialVersion, null)
        {
            if (!String.IsNullOrEmpty(originalString))
                _originalString = originalString;
            else
                _originalString = GetLegacyString(version, specialVersion);

            _version = NormalizeVersionValue(version);
        }

        internal SemanticVersion(SemanticVersion semVer)
            : base(semVer.Version.Major, semVer.Version.Minor, semVer.Version.Build, semVer.PrereleaseLabels, semVer.Metadata)
        {
            _originalString = semVer.ToString();
            _version = semVer.Version;
        }

        /// <summary>
        /// Gets the normalized version portion.
        /// </summary>
        public Version Version
        {
            get
            {
                if (_version == null)
                {
                    return new Version(Major, Minor, Patch, 0);
                }

                return _version;
            }
        }

        public bool IsLegacyVersion
        {
            get
            {
                return _version != null && _version.Revision > 0;
            }
        }

        public string[] GetOriginalVersionComponents()
        {
            if (!String.IsNullOrEmpty(_originalString))
            {
                string original;

                // search the start of the SpecialVersion part, if any
                int dashIndex = _originalString.IndexOf('-');
                if (dashIndex != -1)
                {
                    // remove the SpecialVersion part
                    original = _originalString.Substring(0, dashIndex);
                }
                else
                {
                    original = _originalString;
                }

                return SplitAndPadVersionString(original);
            }
            else
            {
                return SplitAndPadVersionString(Version.ToString());
            }
        }

        private static string[] SplitAndPadVersionString(string version)
        {
            string[] a = version.Split('.');
            if (a.Length == 4)
            {
                return a;
            }
            else
            {
                // if 'a' has less than 4 elements, we pad the '0' at the end 
                // to make it 4.
                var b = new string[4] { "0", "0", "0", "0"};
                Array.Copy(a, 0, b, 0, a.Length);
                return b;
            }
        }

        /// <summary>
        /// Parses a version string using loose semantic versioning rules that allows 2-4 version components followed by an optional special version.
        /// </summary>
        public static SemanticVersion Parse(string version)
        {
            if (String.IsNullOrEmpty(version))
            {
                throw new ArgumentException(Resources.Argument_Cannot_Be_Null_Or_Empty, "version");
            }

            SemanticVersion semVer;
            if (!TryParse(version, out semVer))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.InvalidVersionString, version), "version");
            }

            return semVer;
        }

        /// <summary>
        /// Parses a version string using loose semantic versioning rules that allows 2-4 version components followed by an optional special version.
        /// </summary>
        public static bool TryParse(string version, out SemanticVersion value)
        {
            value = null;
            if (String.IsNullOrEmpty(version))
            {
                return false;
            }

            var match = _semanticVersionRegex.Match(version.Trim());
            Version versionValue;
            if (!match.Success || !Version.TryParse(match.Groups["Version"].Value, out versionValue))
            {
                return false;
            }

            Version ver = NormalizeVersionValue(versionValue);

            value = new SemanticVersion(ver, match.Groups["Release"].Value.TrimStart('-'), version.Replace(" ", ""));
            return true;
        }

        /// <summary>
        /// Parses a version string using strict semantic versioning rules that allows exactly 3 components and an optional special version.
        /// </summary>
        public static bool TryParseStrict(string version, out SemanticVersion value)
        {
            SemanticVersionStrict val;
            if (SemanticVersionStrict.TryParse(version, out val))
            {
                value = new SemanticVersion(val);
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Attempts to parse the version token as a SemanticVersion.
        /// </summary>
        /// <returns>An instance of SemanticVersion if it parses correctly, null otherwise.</returns>
        public static SemanticVersion ParseOptionalVersion(string version)
        {
            return Parse(version);
        }


        public static bool operator ==(SemanticVersion version1, SemanticVersion version2)
        {
            if (Object.ReferenceEquals(version1, null))
            {
                return Object.ReferenceEquals(version2, null);
            }

            return version1.Equals(version2);
        }

        public static bool operator !=(SemanticVersion version1, SemanticVersion version2)
        {
            return !(version1 == version2);
        }

        public static bool operator <(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1 == null)
            {
                throw new ArgumentNullException("version1");
            }
            return version1.CompareTo(version2) < 0;
        }

        public static bool operator <=(SemanticVersion version1, SemanticVersion version2)
        {
            return (version1 == version2) || (version1 < version2);
        }

        public static bool operator >(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1 == null)
            {
                throw new ArgumentNullException("version1");
            }

            return version2 < version1;
        }

        public static bool operator >=(SemanticVersion version1, SemanticVersion version2)
        {
            return (version1 == version2) || (version1 > version2);
        }

        public override string ToString()
        {
            if (_originalString == null)
            {
                return GetLegacyString(Version, SpecialVersion);
            }

            return _originalString;
        }

        public override string ToNormalizedString()
        {
            if (IsLegacyVersion)
            {
                return GetLegacyString(Version, SpecialVersion);
            }

            return base.ToNormalizedString();
        }

        private static string GetLegacyString(Version version, string specialVersion)
        {
            return version.ToString() + (!String.IsNullOrEmpty(specialVersion) ? "-" + specialVersion : null);
        }

        public override int GetHashCode()
        {
            return ToNormalizedString().GetHashCode();
        }

        public int CompareTo(ISemanticVersion other)
        {
            SemanticVersionComparer comparer = new SemanticVersionComparer();
            return comparer.Compare(this, other);
        }

        public bool Equals(ISemanticVersion other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ISemanticVersion);
        }

        public override int CompareTo(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return 1;
            }

            SemanticVersion other = obj as SemanticVersion;
            if (other == null)
            {
                throw new ArgumentException(Resources.TypeMustBeASemanticVersion, "obj");
            }
            return CompareTo(other);
        }
    }
}
