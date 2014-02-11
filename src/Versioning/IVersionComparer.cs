using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGet.Versioning
{
    /// <summary>
    /// IVersionComparer represents a version comparer capable of sorting and determining the equality of ISemanticVersion objects.
    /// </summary>
    public interface IVersionComparer : IEqualityComparer<ISemanticVersion>, IComparer<ISemanticVersion>
    {

    }
}
