using NuGet.Versioning;
using System;
using System.Collections.Generic;

namespace NuGet
{
    public class DefaultConstraintProvider : IPackageConstraintProvider
    {
        private readonly Dictionary<string, NuGetVersionRange> _constraints = new Dictionary<string, NuGetVersionRange>(StringComparer.OrdinalIgnoreCase);

        public string Source
        {
            get
            {
                return String.Empty;
            }
        }

        public void AddConstraint(string packageId, NuGetVersionRange versionRange)
        {
            _constraints[packageId] = versionRange;
        }

        public NuGetVersionRange GetConstraint(string packageId)
        {
            NuGetVersionRange versionRange;
            if (_constraints.TryGetValue(packageId, out versionRange))
            {
                return versionRange;
            }
            return null;
        }
    }
}
