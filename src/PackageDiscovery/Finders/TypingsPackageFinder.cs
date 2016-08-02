using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;

namespace PackageDiscovery.Finders
{
    [Export(Moniker, typeof(IReferencedPackageFinder))]
    public sealed class TypingsPackageFinder : IReferencedPackageFinder
    {
        public const string Moniker = "Typings";

        public IReadOnlyCollection<Package> FindReferencedPackages(DirectoryInfo directory)
        {
            return directory
                .GetFiles("typings.json", SearchOption.AllDirectories)
                .Select(f => JObject.Parse(File.ReadAllText(f.FullName)))
                .SelectMany(j =>
                    new[] {
                        j.Value<JObject>("dependencies"),
                        j.Value<JObject>("devDependencies"),
                        j.Value<JObject>("peerDependencies"),
                        j.Value<JObject>("globalDependencies"),
                        j.Value<JObject>("ambientDependencies"),
                        j.Value<JObject>("ambientDevDependencies"),
                    }
                        .Select(n => n?.Properties())
                        .Where(s => s != null)
                        .Concat()
                )
                .Select(j => new Package(
                    Moniker,
                    j.Name,
                    GetVersion(j.Value.ToString()),
                    (j.Parent.Parent as JProperty).Name.IndexOf("devDependencies", StringComparison.OrdinalIgnoreCase) != -1
                ))
                .Distinct(p => new { p.Id, p.Version })
                .OrderBy(p => p.Id)
                .ThenBy(p => p.Version)
                .ToList();
        }

        private static string GetVersion(string versionString)
        {
            string[] parts = versionString.Split('#');

            if (parts.Length <= 1)
                return null;

            return parts.Last();
        }
    }
}
