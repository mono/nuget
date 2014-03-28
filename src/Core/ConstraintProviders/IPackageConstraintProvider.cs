using NuGet.Versioning;
namespace NuGet
{
    public interface IPackageConstraintProvider
    {
        string Source { get; }
        NuGetVersionRange GetConstraint(string packageId);
    }
}
