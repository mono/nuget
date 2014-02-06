using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGet.Versioning
{
    public interface INuGetVersion : ISemanticVersion, IComparable<INuGetVersion>, IEquatable<INuGetVersion>
    {
        Version Version { get; }

        bool IsLegacyVersion { get; }

        bool Equals(INuGetVersion other, VersionComparison versionComparison);

        int CompareTo(INuGetVersion other, VersionComparison versionComparison);
    }
}
