using NuGet.Server.DataServices;
using NuGet.Versioning;

namespace NuGet.Server.Infrastructure
{
    public interface IServerPackageRepository : IServiceBasedRepository
    {
        void RemovePackage(string packageId, SemanticVersion version);
        Package GetMetadataPackage(IPackage package);
    }
}
