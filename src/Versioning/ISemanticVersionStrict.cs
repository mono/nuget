using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGet.Versioning
{
    public interface ISemanticVersionStrict : IComparable
    {
        int Major { get; }
        int Minor { get; }
        int Patch { get; }

        IEnumerable<string> ReleaseLabels { get; }

        string SpecialVersion { get; }

        bool IsPrerelease { get; }

        bool  HasMetadata { get; }

        string Metadata { get; }

        string ToNormalizedString();
    }
}
