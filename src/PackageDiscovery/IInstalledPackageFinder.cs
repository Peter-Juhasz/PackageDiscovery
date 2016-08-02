using System.Collections.Generic;
using System.IO;

namespace PackageDiscovery
{
    public interface IInstalledPackageFinder
    {
        IReadOnlyCollection<Package> FindInstalledPackages(DirectoryInfo directory);
    }
}
