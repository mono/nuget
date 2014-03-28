using NuGet.Versioning;
using System;
using System.IO;

namespace NuGet
{
    public interface IPackageCacheRepository : IPackageRepository
    {
        bool InvokeOnPackage(string packageId, NuGetVersion version, Action<Stream> action);
    }
}