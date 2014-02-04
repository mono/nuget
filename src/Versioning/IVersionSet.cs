using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGet.Versioning
{
    public interface IVersionSet : IEnumerable<IVersion>, IFormattable
    {

    }
}
