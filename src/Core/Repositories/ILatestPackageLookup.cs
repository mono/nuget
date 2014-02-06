using NuGet.Versioning;

namespace NuGet
{
    public interface ILatestPackageLookup
    {
        bool TryFindLatestPackageById(string id, out ISemanticVersion latestVersion);
        bool TryFindLatestPackageById(string id, bool includePrerelease, out IPackage package);
    }
}