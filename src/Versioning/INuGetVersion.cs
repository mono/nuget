using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGet.Versioning
{
    /// <summary>
    /// A hybrid model of SemVer that supports both semantic versioning as described at http://semver.org, and older 4-digit versioning schemes.
    /// </summary>
    public interface INuGetVersion : ISemanticVersion, IComparable<INuGetVersion>, IEquatable<INuGetVersion>
    {
        /// <summary>
        /// A System.Version representation of the version without metadata or release labels.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// True if the INuGetVersion is using legacy behavior.
        /// </summary>
        bool IsLegacyVersion { get; }

        /// <summary>
        /// True if the INuGetVersion objects are equal based on the given comparison mode.
        /// </summary>
        bool Equals(INuGetVersion other, VersionComparison versionComparison);

        /// <summary>
        /// Compares INuGetVersion objects using the given comparison mode.
        /// </summary>
        int CompareTo(INuGetVersion other, VersionComparison versionComparison);
    }
}
