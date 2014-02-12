namespace NuGet.Versioning
{
    /// <summary>
    /// IVersionSpec represents a range of allowed versions.
    /// </summary>
    public interface IVersionSpec
    {
        /// <summary>
        /// The minimum version allowed.
        /// </summary>
        ISemanticVersion MinVersion { get; }

        /// <summary>
        /// True if MinVersion should be included in the range.
        /// </summary>
        bool IsMinInclusive { get; }

        /// <summary>
        /// The maximum version allowed.
        /// </summary>
        ISemanticVersion MaxVersion { get; }

        /// <summary>
        /// True if MaxVersion should be included in the range.
        /// </summary>
        bool IsMaxInclusive { get; }

        /// <summary>
        /// Determines if an ISemanticVersion meets the requirements.
        /// </summary>
        /// <param name="version">SemVer to compare</param>
        /// <returns>True if the given version meets the version requirements.</returns>
        bool Satisfies(ISemanticVersion version);

        /// <summary>
        /// Determines if an ISemanticVersion meets the requirements using the given mode.
        /// </summary>
        /// <param name="version">SemVer to compare</param>
        /// <param name="versionComparison">VersionComparison mode used to determine the version range.</param>
        /// <returns>True if the given version meets the version requirements.</returns>
        bool Satisfies(ISemanticVersion version, VersionComparison versionComparison);

        /// <summary>
        /// Determines if an ISemanticVersion meets the requirements using the version comparer.
        /// </summary>
        /// <param name="version">SemVer to compare.</param>
        /// <param name="comparer">Version comparer used to determine if the version criteria is met.</param>
        /// <returns>True if the given version meets the version requirements.</returns>
        bool Satisfies(ISemanticVersion version, IVersionComparer comparer);

        /// <summary>
        /// A pretty print representation of the IVersionSpec.
        /// </summary>
        string PrettyPrint();

        /// <summary>
        /// Returns a string representing the IVersionSpec with normalized versions.
        /// </summary>
        string ToNormalizedString();
    }
}
