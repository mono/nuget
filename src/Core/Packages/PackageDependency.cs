using NuGet.Versioning;
using System;

namespace NuGet
{
    public class PackageDependency
    {
        public PackageDependency(string id)
            : this(id, versionRange: null)
        {
        }

        public PackageDependency(string id, NuGetVersionRange versionRange) 
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "id");
            }
            Id = id;
            VersionRange = versionRange;
        }

        public string Id
        {
            get;
            private set;
        }

        public NuGetVersionRange VersionRange
        {
            get;
            private set;
        }

        public override string ToString()
        {
            if (VersionRange == null)
            {
                return Id;
            }

            return Id + " " + VersionRange.PrettyPrint();
        }

        internal static PackageDependency CreateDependency(string id, string versionRange)
        {
            return new PackageDependency(id, NuGetVersionRange.Parse(versionRange));
        }
    }
}