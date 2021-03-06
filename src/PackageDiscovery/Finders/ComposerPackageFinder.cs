﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;

namespace PackageDiscovery.Finders
{
    [Export(Moniker, typeof(IReferencedPackageFinder))]
    public sealed class ComposerPackageFinder : IReferencedPackageFinder
    {
        public const string Moniker = "Composer";

        public IReadOnlyCollection<Package> FindReferencedPackages(DirectoryInfo directory)
        {
            return directory
                .GetFiles("composer.json", SearchOption.AllDirectories)
                .Select(f => JObject.Parse(File.ReadAllText(f.FullName)))
                .SelectMany(j => j.Value<JObject>("require")?.Properties() ?? Enumerable.Empty<JProperty>())
                .Select(j => new Package(
                    Moniker,
                    j.Name,
                    j.Value.ToString()
                ))
                .Distinct(p => new { p.Id, p.Version })
                .OrderBy(p => p.Id)
                .ThenBy(p => p.Version)
                .ToList();
        }
    }
}
