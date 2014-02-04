using System;

namespace NuGet.Versioning
{
    public interface IVersionSpec : IVersionSet
    {
        SemanticVersion MinVersion { get; }
        bool IsMinInclusive { get; }
        SemanticVersion MaxVersion { get; }
        bool IsMaxInclusive { get; }

        bool Satisfies(SemanticVersion version);
    }
}
