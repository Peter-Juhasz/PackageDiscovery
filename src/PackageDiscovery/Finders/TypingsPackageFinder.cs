﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PackageDiscovery.Finders
{
    public sealed class TypingsPackageFinder : IPackageFinder
    {
        public const string Moniker = "Typings";

        public IReadOnlyCollection<Package> FindPackages(DirectoryInfo directory)
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
                    j.Value.ToString(),
                    (j.Parent.Parent as JProperty).Name.IndexOf("devDependencies", StringComparison.OrdinalIgnoreCase) != -1
                ))
                .Distinct(p => new { p.Id, p.Version })
                .OrderBy(p => p.Id)
                .ThenBy(p => p.Version)
                .ToList();
        }
    }
}