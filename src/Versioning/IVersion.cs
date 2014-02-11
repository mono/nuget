using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGet.Versioning
{
    /// <summary>
    /// A basic version that allows comparisons.
    /// </summary>
    public interface IVersion : IComparable
    {
        string ToNormalizedString();
    }
}
