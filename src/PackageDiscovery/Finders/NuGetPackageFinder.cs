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
            var packagesFromRepositories = (
                from f in directory
                    .GetFiles("repositories.config", SearchOption.AllDirectories)
                let x = XDocument.Load(f.FullName)
                from r in x.Root.Elements("repository")
                select new FileInfo(Path.Combine(f.Directory.FullName, r.Attribute("path").Value))
            );

            var standalonePackages = directory
                .GetFiles("packages.config", SearchOption.AllDirectories);

            return standalonePackages.Union(packagesFromRepositories)
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
