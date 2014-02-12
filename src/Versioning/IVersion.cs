using System;

namespace NuGet.Versioning
{
    /// <summary>
    /// A basic version that allows comparisons.
    /// </summary>
    public interface IVersion : IComparable
    {
        /// <summary>
        /// Gives a normalized representation of the version.
        /// </summary>
        string ToNormalizedString();
    }
}
