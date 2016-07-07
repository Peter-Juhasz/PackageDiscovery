using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PackageDiscovery.Finders
{
    public sealed class TsdPackageFinder : IPackageFinder
    {
        public const string Moniker = "TSD";

        public IReadOnlyCollection<Package> FindPackages(DirectoryInfo directory)
        {
            return directory
                .GetFiles("tsd.json", SearchOption.AllDirectories)
                .Select(f => JObject.Parse(File.ReadAllText(f.FullName)))
                .SelectMany(j => j.Value<JObject>("installed")?.Properties() ?? Enumerable.Empty<JProperty>())
                .Select(j => new Package(
                    Moniker,
                    j.Name,
                    j.Value["commit"].ToString()
                ))
                .Distinct(p => new { p.Id, p.Version })
                .OrderBy(p => p.Id)
                .ThenBy(p => p.Version)
                .ToList();
        }
    }
}
