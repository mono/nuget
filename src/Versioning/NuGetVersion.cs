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
    public sealed class NuGetVersion : INuGetVersion
    {
        private const RegexOptions _flags = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
        private static readonly Regex _semanticVersionRegex = new Regex(@"^(?<Version>\d+(\s*\.\s*\d+){0,3})(?<Release>-[a-z][0-9a-z-]*)?(?<Metadata>\+[0-9A-Za-z-]+)?$", _flags);
        private static readonly Regex _strictSemanticVersionRegex = new Regex(@"^(?<Version>([0-9]|[1-9][0-9]*)(\.([0-9]|[1-9][0-9]*)){2})(?<Release>-[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)?(?<Metadata>\+[0-9A-Za-z-]+)?$", _flags);
        private readonly string _originalString;
        private Version _version;
        private readonly List<string> _releaseLabels;
        private readonly string _metadata;

        public NuGetVersion(string version)
            : this(Parse(version))
        {
            // The constructor normalizes the version string so that it we do not need to normalize it every time we need to operate on it. 
            // The original string represents the original form in which the version is represented to be used when printing.
            _originalString = version;
        }

        public NuGetVersion(int major, int minor, int build, int revision)
            : this(new Version(major, minor, build, revision))
        { }

        public NuGetVersion(int major, int minor, int patch, IEnumerable<string> releaseLabels, string metadata)
            : this(new Version(major, minor, patch), new List<string>(releaseLabels), metadata, null)
        { }

        public NuGetVersion(Version version)
            : this(version, String.Empty)
        { }

        public NuGetVersion(Version version, string specialVersion)
            : this(version, specialVersion, null, null)
        { }

        private NuGetVersion(Version version, string specialVersion, string metadata, string originalString)
            : this(version, new List<string>() { specialVersion }, metadata, originalString)
        { }

        private NuGetVersion(Version version, List<string> releaseLabels, string metadata, string originalString)
        {
            _metadata = metadata;

            _releaseLabels = releaseLabels;

            if (releaseLabels == null || releaseLabels.All(s => String.IsNullOrEmpty(s)))
            {
                 _releaseLabels = new List<string>(0);
            }

            if (!String.IsNullOrEmpty(originalString))
            {
                _originalString = originalString;
            }
            else
            {
                _originalString = GetLegacyString(version, _releaseLabels);
            }

            _version = NormalizeVersionValue(version);
        }

        internal NuGetVersion(NuGetVersion semVer)
            : this(semVer.Version, new List<string>(semVer.ReleaseLabels), semVer.Metadata, semVer.ToString())
        { }

        public int Major { get { return Version.Major; } }
        public int Minor { get { return Version.Minor; } }
        public int Patch { get { return Version.Build; } }

        public IEnumerable<string> ReleaseLabels
        {
            get
            {
                return _releaseLabels;
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
                    return String.Join(".", _releaseLabels);
                }

                return string.Empty;
            }
        }

        public bool IsPrerelease
        {
            get
            {
                return _releaseLabels.Count > 0;
            }
        }

        public bool HasMetadata
        {
            get
            {
                return !String.IsNullOrEmpty(Metadata);
            }
        }

        public string Metadata
        {
            get
            {
                return _metadata;
            }
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
                    _version = new Version(Major, Minor, Patch, 0);
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

        /// <summary>
        /// Parses a version string using loose semantic versioning rules that allows 2-4 version components followed by an optional special version.
        /// </summary>
        public static NuGetVersion Parse(string version)
        {
            if (String.IsNullOrEmpty(version))
            {
                throw new ArgumentException(Resources.Argument_Cannot_Be_Null_Or_Empty, "version");
            }

            NuGetVersion semVer;
            if (!TryParse(version, out semVer))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.InvalidVersionString, version), "version");
            }

            return semVer;
        }

        /// <summary>
        /// Parses a version string using loose semantic versioning rules that allows 2-4 version components followed by an optional special version.
        /// </summary>
        public static bool TryParse(string version, out NuGetVersion value)
        {
            return TryParseInternal(version, _semanticVersionRegex, out value);
        }

        /// <summary>
        /// Parses a version string using strict semantic versioning rules that allows exactly 3 components and an optional special version.
        /// </summary>
        public static bool TryParseStrict(string version, out NuGetVersion value)
        {
            return TryParseInternal(version, _strictSemanticVersionRegex, out value);
        }

        private static bool TryParseInternal(string version, Regex regex, out NuGetVersion value)
        {
            if (!String.IsNullOrEmpty(version))
            {
                var match = regex.Match(version.Trim());

                Version versionValue;
                if (match.Success && Version.TryParse(match.Groups["Version"].Value, out versionValue))
                {
                    Version ver = NormalizeVersionValue(versionValue);

                    value = new NuGetVersion(version: ver, 
                                                specialVersion: match.Groups["Release"].Value.TrimStart('-'), 
                                                metadata: match.Groups["Metadata"].Value.TrimStart('+'),
                                                originalString: version.Replace(" ", ""));
                    return true;
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Attempts to parse the version token as a SemanticVersion.
        /// </summary>
        /// <returns>An instance of SemanticVersion if it parses correctly, null otherwise.</returns>
        public static NuGetVersion ParseOptionalVersion(string version)
        {
            NuGetVersion semver = null;
            TryParse(version, out semver);
            return semver;
        }

        public override string ToString()
        {
            if (_originalString == null)
            {
                return GetLegacyString(Version, _releaseLabels);
            }

            return _originalString;
        }

        public string ToNormalizedString()
        {
            if (IsLegacyVersion)
            {
                return GetLegacyString(Version, _releaseLabels);
            }

            return GetStrictSemVerString();
        }

        /*
        public static SemanticVersion MaxValue
        {
            get
            {
                return new SemanticVersion(Int32.MaxValue, Int32.MaxValue, Int32.MaxValue, Int32.MaxValue);
            }
        }

        public static SemanticVersion MinValue
        {
            get
            {
                return new SemanticVersion(new Version(0, 0), "PRE");
            }
        } */

        private string GetStrictSemVerString()
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

        private static string GetLegacyString(Version version, List<string> releaseLabels)
        {
            string specialVersion = String.Join(".", releaseLabels);
            return version.ToString() + (!String.IsNullOrEmpty(specialVersion) ? "-" + specialVersion : null);
        }

        #region Compare

        public override int GetHashCode()
        {
            return ToNormalizedString().ToUpperInvariant().GetHashCode();
        }

        public int CompareTo(INuGetVersion other)
        {
            return Compare(this, other);
        }

        public bool Equals(INuGetVersion other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as INuGetVersion);
        }

        public int CompareTo(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return 1;
            }

            INuGetVersion other = obj as INuGetVersion;
            if (other == null)
            {
                throw new ArgumentException(Resources.TypeMustBeASemanticVersion, "obj");
            }

            return CompareTo(other);
        }

        public bool Equals(INuGetVersion other, VersionComparison versionComparison)
        {
            VersionComparer comparer = new VersionComparer(versionComparison);
            return comparer.Equals(this, other);
        }

        public int CompareTo(INuGetVersion other, VersionComparison versionComparison)
        {
            VersionComparer comparer = new VersionComparer(versionComparison);
            return comparer.Compare(this, other);
        }

        public static bool operator ==(NuGetVersion version1, NuGetVersion version2)
        {
            return Compare(version1, version2) == 0;
        }

        public static bool operator !=(NuGetVersion version1, NuGetVersion version2)
        {
            return Compare(version1, version2) != 0;
        }

        public static bool operator <(NuGetVersion version1, NuGetVersion version2)
        {
            return Compare(version1, version2) < 0;
        }

        public static bool operator <=(NuGetVersion version1, NuGetVersion version2)
        {
            return Compare(version1, version2) <= 0;
        }

        public static bool operator >(NuGetVersion version1, NuGetVersion version2)
        {
            return Compare(version1, version2) > 0;
        }

        public static bool operator >=(NuGetVersion version1, NuGetVersion version2)
        {
            return Compare(version1, version2) >= 0;
        }

        #endregion

        #region Helper methods

        private static int Compare(INuGetVersion version1, INuGetVersion version2)
        {
            IVersionComparer comparer = new VersionComparer();
            return comparer.Compare(version1, version2);
        }

        private static Version NormalizeVersionValue(Version version)
        {
            return new Version(version.Major,
                               version.Minor,
                               Math.Max(version.Build, 0),
                               Math.Max(version.Revision, 0));
        }

        #endregion
    }
}
