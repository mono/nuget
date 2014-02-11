using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGet.Versioning
{
    /// <summary>
    /// ISemanticVersion represents a semantic version 2.0.0 as described at http://semver.org
    /// </summary>
    public interface ISemanticVersion : IVersion, IComparable<ISemanticVersion>, IEquatable<ISemanticVersion>
    {
        /// <summary>
        /// Major version X (X.y.z)
        /// </summary>
        int Major { get; }

        /// <summary>
        /// Minor version Y (x.Y.z)
        /// </summary>
        int Minor { get; }

        /// <summary>
        /// Patch version Z (x.y.Z)
        /// </summary>
        int Patch { get; }

        /// <summary>
        /// A collection of pre-release labels attached to the version.
        /// </summary>
        IEnumerable<string> ReleaseLabels { get; }

        /// <summary>
        /// The full pre-release label for the version.
        /// </summary>
        string SpecialVersion { get; }

        /// <summary>
        /// True if pre-release labels exist for the version.
        /// </summary>
        bool IsPrerelease { get; }

        /// <summary>
        /// True if metadata exists for the version.
        /// </summary>
        bool  HasMetadata { get; }

        /// <summary>
        /// Build metadata attached to the version.
        /// </summary>
        string Metadata { get; }

        /// <summary>
        /// True if the ISemanticVersion objects are equal based on the given comparison mode.
        /// </summary>
        bool Equals(ISemanticVersion other, VersionComparison versionComparison);

        /// <summary>
        /// Compares ISemanticVersion objects using the given comparison mode.
        /// </summary>
        int CompareTo(ISemanticVersion other, VersionComparison versionComparison);
    }
}
