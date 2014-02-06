using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NuGet
{
    public static class VersionExtensions
    {
        public static Func<IPackage, bool> ToDelegate(this IVersionSpec versionInfo)
        {
            if (versionInfo == null)
            {
                throw new ArgumentNullException("versionInfo");
            }
            return versionInfo.ToDelegate<IPackage>(p => p.Version);
        }

        public static Func<T, bool> ToDelegate<T>(this IVersionSpec versionInfo, Func<T, ISemanticVersion> extractor)
        {
            if (versionInfo == null)
            {
                throw new ArgumentNullException("versionInfo");
            }
            if (extractor == null)
            {
                throw new ArgumentNullException("extractor");
            }

            return p =>
            {
                ISemanticVersion version = extractor(p);
                bool condition = true;
                if (versionInfo.MinVersion != null)
                {
                    if (versionInfo.IsMinInclusive)
                    {
                        condition = condition && version.CompareTo(versionInfo.MinVersion) >= 0;
                    }
                    else
                    {
                        condition = condition && version.CompareTo(versionInfo.MinVersion) > 0;
                    }
                }

                if (versionInfo.MaxVersion != null)
                {
                    if (versionInfo.IsMaxInclusive)
                    {
                        condition = condition && version.CompareTo(versionInfo.MaxVersion) <= 0;
                    }
                    else
                    {
                        condition = condition && version.CompareTo(versionInfo.MaxVersion) < 0;
                    }
                }

                return condition;
            };
        }

        public static string[] GetOriginalVersionComponents(this ISemanticVersion version)
        {
            string originalString = version.ToString();

            string original = string.Empty;

            // search the start of the SpecialVersion part, if any
            int dashIndex = originalString.IndexOfAny(new char[] { '-', '+' });
            if (dashIndex != -1)
            {
                // remove the SpecialVersion part
                original = originalString.Substring(0, dashIndex);
            }
            else
            {
                original = originalString;
            }

            return SplitAndPadVersionString(original);
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
                var b = new string[4] { "0", "0", "0", "0" };
                Array.Copy(a, 0, b, 0, a.Length);
                return b;
            }
        }

        public static Version GetLegacyVersion(this ISemanticVersion version)
        {
            INuGetVersion legacyVersion = version as INuGetVersion;
            if (legacyVersion != null)
            {
                return legacyVersion.Version;
            }

            return new Version(version.Major, version.Minor, version.Patch);
        }


        public static IEnumerable<string> GetComparableVersionStrings(this ISemanticVersion version)
        {
            Version coreVersion = version.GetLegacyVersion();
            string specialVersion = String.IsNullOrEmpty(version.SpecialVersion) ? String.Empty : "-" + version.SpecialVersion;

            string originalVersion = version.ToString();
            string[] originalVersionComponents = version.GetOriginalVersionComponents();

            var paths = new LinkedList<string>();

            if (coreVersion.Revision == 0)
            {
                if (coreVersion.Build == 0)
                {
                    string twoComponentVersion = String.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}{2}",
                        originalVersionComponents[0],
                        originalVersionComponents[1],
                        specialVersion);

                    AddVersionToList(originalVersion, paths, twoComponentVersion);
                }

                string threeComponentVersion = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.{1}.{2}{3}",
                    originalVersionComponents[0],
                    originalVersionComponents[1],
                    originalVersionComponents[2],
                    specialVersion);

                AddVersionToList(originalVersion, paths, threeComponentVersion);
            }

            string fullVersion = String.Format(
                   CultureInfo.InvariantCulture,
                   "{0}.{1}.{2}.{3}{4}",
                   originalVersionComponents[0],
                   originalVersionComponents[1],
                   originalVersionComponents[2],
                   originalVersionComponents[3],
                   specialVersion);

            AddVersionToList(originalVersion, paths, fullVersion);

            return paths;
        }

        private static void AddVersionToList(string originalVersion, LinkedList<string> paths, string nextVersion)
        {
            if (nextVersion.Equals(originalVersion, StringComparison.OrdinalIgnoreCase))
            {
                // IMPORTANT: we want to put the original version string first in the list. 
                // This helps the DataServicePackageRepository reduce the number of requests
                // int the Exists() and FindPackage() methods.
                paths.AddFirst(nextVersion);
            }
            else
            {
                paths.AddLast(nextVersion);
            }
        }
    }
}
