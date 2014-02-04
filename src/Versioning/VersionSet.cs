using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGet.Versioning
{
    public class VersionSet : IVersionSet
    {
        private readonly IEnumerable<IVersion> _set;

        public VersionSet()
            : this(Enumerable.Empty<IVersion>())
        {

        }

        public VersionSet(IVersion version)
            : this(new List<IVersion>() { version })
        {

        }

        public VersionSet(IEnumerable<IVersion> set)
        {
            _set = set;
        }

        public virtual IEnumerator<IVersion> GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
