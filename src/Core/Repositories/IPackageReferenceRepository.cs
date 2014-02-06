using NuGet.Versioning;
using System.Runtime.Versioning;

namespace NuGet
{
    public interface IPackageReferenceRepository : IPackageRepository
    {
        void AddPackage(string packageId, ISemanticVersion version, bool developmentDependency, FrameworkName targetFramework);
        FrameworkName GetPackageTargetFramework(string packageId);
    }
}
