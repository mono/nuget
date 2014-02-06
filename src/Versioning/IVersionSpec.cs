using System;

namespace NuGet.Versioning
{
    public interface IVersionSpec
    {
        ISemanticVersion MinVersion { get; }
        bool IsMinInclusive { get; }
        ISemanticVersion MaxVersion { get; }
        bool IsMaxInclusive { get; }

        bool Satisfies(ISemanticVersion version);
    }

    public interface IVersionSpec<T> where T : IVersion
    {
        T MinVersion { get; }
        bool IsMinInclusive { get; }
        T MaxVersion { get; }
        bool IsMaxInclusive { get; }

        bool Satisfies(T version);
    }
}
