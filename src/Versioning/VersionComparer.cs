using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NuGet.Versioning
{
    public sealed class VersionComparer : IVersionComparer<ISemanticVersion>
    {
        private VersionComparison _mode;

        /// <summary>
        /// Creates a VersionComparer using the default mode.
        /// </summary>
        public VersionComparer()
        {
            _mode = VersionComparison.Default;
        }

        /// <summary>
        /// Creates a VersionComparer that respects the given comparison mode.
        /// </summary>
        /// <param name="versionComparison">comparison mode</param>
        public VersionComparer(VersionComparison versionComparison)
        {
            _mode = versionComparison;
        }

        public bool Equals(ISemanticVersion x, ISemanticVersion y)
        {
            return Compare(x, y) == 0;
        }

        public static int Compare(ISemanticVersion version1, ISemanticVersion version2, VersionComparison versionComparison)
        {
            IVersionComparer<ISemanticVersion> comparer = new VersionComparer(versionComparison);
            return comparer.Compare(version1, version2);
        }

        public int GetHashCode(ISemanticVersion obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return 0;
            }

            string verString = string.Empty;

            if (_mode == VersionComparison.Strict)
            {
                verString = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", obj.Version.ToString(), obj.SpecialVersion);
            }
            else if (_mode == VersionComparison.IgnoreMetadata)
            {
                verString = String.Format(CultureInfo.InvariantCulture, "{0}-{1}", obj.Version.ToString(), obj.SpecialVersion.ToUpperInvariant());
            }
            else if (_mode == VersionComparison.Version)
            {
                verString = obj.Version.ToString();
            }
            else
            {
                verString = obj.ToNormalizedString().ToUpperInvariant();
            }

            return verString.GetHashCode();
        }

        public int Compare(ISemanticVersion x, ISemanticVersion y)
        {
            // null checks
            if (Object.ReferenceEquals(x, null) && Object.ReferenceEquals(y, null))
            {
                return 0;
            }

            if (Object.ReferenceEquals(y, null))
            {
                return 1;
            }

            if (Object.ReferenceEquals(x, null))
            {
                return -1;
            }

            // compare version
            int result = x.Major.CompareTo(y.Major);
            if (result != 0)
                return result;

            result = x.Minor.CompareTo(y.Minor);
            if (result != 0)
                return result;

            result = x.Patch.CompareTo(y.Patch);
            if (result != 0)
                return result;

            if (_mode != VersionComparison.Strict)
            {
                // true if one has a 4th version number
                if (x.IsLegacyVersion || y.IsLegacyVersion)
                {
                    result = x.Version.CompareTo(y.Version);
                    if (result != 0)
                        return result;
                }
            }

            if (_mode != VersionComparison.Version)
            {
                // compare release labels
                if (x.IsPrerelease && !y.IsPrerelease)
                    return -1;

                if (!x.IsPrerelease && y.IsPrerelease)
                    return 1;

                if (x.IsPrerelease && y.IsPrerelease)
                {
                    result = CompareReleaseLabels(x.ReleaseLabels, y.ReleaseLabels);
                    if (result != 0)
                        return result;
                }

                // compare the metadata
                if (_mode != VersionComparison.IgnoreMetadata && _mode != VersionComparison.Strict)
                {
                    result = StringComparer.OrdinalIgnoreCase.Compare(x.Metadata ?? string.Empty, y.Metadata ?? string.Empty);
                    if (result != 0)
                        return result;
                }
            }

            return 0;
        }

        public static IVersionComparer<ISemanticVersion> Version
        {
            get
            {
                return new VersionComparer(VersionComparison.Version);
            }
        }

        public static IVersionComparer<ISemanticVersion> IgnoreMetadata
        {
            get
            {
                return new VersionComparer(VersionComparison.IgnoreMetadata);
            }
        }

        public static IVersionComparer<ISemanticVersion> Strict
        {
            get
            {
                return new VersionComparer(VersionComparison.Strict);
            }
        }

        private int CompareReleaseLabels(IEnumerable<string> version1, IEnumerable<string> version2)
        {
            int result = 0;

            IEnumerator<string> a = version1.GetEnumerator();
            IEnumerator<string> b = version2.GetEnumerator();

            bool aExists = a.MoveNext();
            bool bExists = b.MoveNext();

            while (aExists || bExists)
            {
                if (!aExists && bExists)
                    return -1;

                if (aExists && !bExists)
                    return 1;

                // compare the labels
                result = CompareRelease(a.Current, b.Current);

                if (result != 0)
                    return result;

                aExists = a.MoveNext();
                bExists = b.MoveNext();
            }

            return result;
        }

        private int CompareRelease(string version1, string version2)
        {
            int version1Num = 0;
            int version2Num = 0;
            int result = 0;

            // if both are numeric compare them as numbers
            if (Int32.TryParse(version1, out version1Num) && Int32.TryParse(version2, out version2Num))
            {
                result = version1Num.CompareTo(version2Num);
            }
            else if (_mode == VersionComparison.Strict)
            {
                // case sensitive
                result = StringComparer.Ordinal.Compare(version1, version2);
            }
            else
            {
                result = StringComparer.OrdinalIgnoreCase.Compare(version1, version2);
            }

            return result;
        }
    }
}
