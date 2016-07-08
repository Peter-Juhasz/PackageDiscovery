﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PackageDiscovery.Finders
{
    public sealed class CocoaPodsPackageFinder : IPackageFinder
    {
        public const string Moniker = "CocoaPods";

        private static readonly Regex PodRegex = new Regex(
            @"\bpod\s*'(?<id>[^']+)'(\s*,\s*'(?<version>[^']+)')?"
        );

        public IReadOnlyCollection<Package> FindPackages(DirectoryInfo directory)
        {
            return directory
                .GetFiles("Podfile", SearchOption.AllDirectories)
                .Select(f => File.ReadAllText(f.FullName))
                .SelectMany(c => PodRegex.Matches(c).Cast<Match>())
                .Select(m => new Package(
                    Moniker,
                    m.Groups["id"].Value,
                    m.Groups["version"].Value
                ))
                .Distinct(p => new { p.Id, p.Version })
                .OrderBy(p => p.Id)
                .ThenBy(p => p.Version)
                .ToList();
        }
    }
}
