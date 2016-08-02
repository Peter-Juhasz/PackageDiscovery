using System.Collections.Generic;
using System.IO;

namespace PackageDiscovery
{
    public interface IReferencedPackageFinder
    {
        IReadOnlyCollection<Package> FindReferencedPackages(DirectoryInfo directory);
    }
}
