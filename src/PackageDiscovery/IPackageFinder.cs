using System.Collections.Generic;
using System.IO;

namespace PackageDiscovery
{
    public interface IPackageFinder
    {
        IReadOnlyCollection<Package> FindPackages(DirectoryInfo directory);
    }
}
