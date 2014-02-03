using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGet.Versioning
{
    public enum VersionComparison
    {
        /// <summary>
        /// Uses the default comparison method.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Strict semantic version 2.0.0 comparison. http://semver.org/spec/v2.0.0.html
        /// </summary>
        Strict = 1,

        /// <summary>
        /// Ignores all metadata during the compare.
        /// </summary>
        IgnoreMetadata = 2,

        /// <summary>
        /// Compares only the version numbers.
        /// </summary>
        Version = 3
    }
}
