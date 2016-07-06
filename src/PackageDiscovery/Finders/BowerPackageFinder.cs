using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PackageDiscovery.Finders
{
    public sealed class BowerPackageFinder : IPackageFinder
    {
        public const string Moniker = "Bower";

        public IReadOnlyCollection<Package> FindPackages(DirectoryInfo directory)
        {
            return directory
                .GetFiles("bower.json", SearchOption.AllDirectories)
                .Select(f => JObject.Parse(File.ReadAllText(f.FullName)))
                .SelectMany(j =>
                    j.Value<JObject>("dependencies")?.Properties().Union(
                        j.Value<JObject>("devDependencies")?.Properties() ?? Enumerable.Empty<JProperty>()
                    ) ?? Enumerable.Empty<JProperty>()
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
    }
}
