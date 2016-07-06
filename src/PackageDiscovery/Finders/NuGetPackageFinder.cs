using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace PackageDiscovery.Finders
{
    public sealed class NuGetPackageFinder : IPackageFinder
    {
        public const string Moniker = "NuGet";

        public IReadOnlyCollection<Package> FindPackages(DirectoryInfo directory)
        {
            return directory
                .GetFiles("packages.config", SearchOption.AllDirectories)
                .Select(f => XDocument.Load(f.FullName))
                .SelectMany(x => x.Root.Elements("package"))
                .Select(x => new Package(Moniker, x.Attribute("id").Value, x.Attribute("version").Value))
                .Distinct(p => new { p.Id, p.Version, p.IsDevelopmentPackage })
                .OrderBy(p => p.Id)
                .ThenBy(p => p.Version)
                .ToList();
        }
    }
}
