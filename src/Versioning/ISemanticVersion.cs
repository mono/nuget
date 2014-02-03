using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGet.Versioning
{
    public interface ISemanticVersion : ISemanticVersionStrict, IComparable<ISemanticVersion>, IEquatable<ISemanticVersion>
    {
        Version Version { get; }

        bool IsLegacyVersion { get; }

        bool Equals(ISemanticVersion other, VersionComparison versionComparison);

        int CompareTo(ISemanticVersion other, VersionComparison versionComparison);
    }
}
