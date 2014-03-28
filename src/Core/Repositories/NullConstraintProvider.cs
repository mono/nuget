using NuGet.Versioning;
using System;

namespace NuGet
{
    public class NullConstraintProvider : IPackageConstraintProvider
    {
        private static readonly NullConstraintProvider _instance = new NullConstraintProvider();
        public static NullConstraintProvider Instance
        {
            get
            {
                return _instance;
            }
        }

        private NullConstraintProvider()
        {
        }

        public string Source
        {
            get
            {
                return String.Empty;
            }
        }

        public NuGetVersionRange GetConstraint(string packageId)
        {
            return null;
        }
    }
}
