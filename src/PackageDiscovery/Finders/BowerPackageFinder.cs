using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;

namespace PackageDiscovery.Finders
{
    [Export(Moniker, typeof(IReferencedPackageFinder))]
    [Export(Moniker, typeof(IInstalledPackageFinder))]
    public sealed class BowerPackageFinder : IReferencedPackageFinder, IInstalledPackageFinder
    {
        public const string Moniker = "Bower";

        public IReadOnlyCollection<Package> FindReferencedPackages(DirectoryInfo directory)
        {
            return directory
                .GetFiles("bower.json", SearchOption.AllDirectories)
                .Select(f => JObject.Parse(File.ReadAllText(f.FullName)))
                .SelectMany(j =>
                    new[] {
                        j.Value<JObject>("dependencies"),
                        j.Value<JObject>("devDependencies"),
                    }
                        .Select(n => n?.Properties())
                        .Where(s => s != null)
                        .Concat()
                )
                .Select(j => new Package(
                    Moniker,
                    j.Name,
                    j.Value.ToString(),
                    (j.Parent.Parent as JProperty).Name == "devDependencies"
                ))
                .Distinct(p => new { p.Id, p.Version, p.IsDevelopmentPackage })
                .OrderBy(p => p.Id)
                .ThenBy(p => p.Version)
                .ToList();
        }

        public IReadOnlyCollection<Package> FindInstalledPackages(DirectoryInfo directory)
        {
            return directory
                .GetDirectories("bower_components", SearchOption.AllDirectories)
                .SelectMany(d => d.GetFiles("bower.json", SearchOption.AllDirectories))
                .Where(f => f.Directory.Parent.Name == "bower_components")
                .Select(f => JObject.Parse(File.ReadAllText(f.FullName)))
                .Where(j => j["name"] != null)
                .Select(j => new Package(
                    Moniker,
                    j["name"].ToString(),
                    j["version"].ToString(),
                    isDevelopmentPackage: null
                ))
                .Distinct(p => new { p.Id, p.Version })
                .OrderBy(p => p.Id)
                .ThenBy(p => p.Version)
                .ToList();
        }
    }
}
