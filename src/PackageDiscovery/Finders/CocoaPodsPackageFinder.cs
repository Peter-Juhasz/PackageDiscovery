using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PackageDiscovery.Finders
{
    [Export(Moniker, typeof(IReferencedPackageFinder))]
    public sealed class CocoaPodsPackageFinder : IReferencedPackageFinder
    {
        public const string Moniker = "CocoaPods";

        private static readonly Regex PodRegex = new Regex(
            @"\bpod\s*'(?<id>[^']+)'(\s*,\s*'(?<version>[^']+)')?"
        );

        public IReadOnlyCollection<Package> FindReferencedPackages(DirectoryInfo directory)
        {
            return directory
                .GetFiles("Podfile", SearchOption.AllDirectories)
                .Select(f => File.ReadAllText(f.FullName))
                .SelectMany(c => PodRegex.Matches(c).Cast<Match>())
                .Select(m => new Package(
                    Moniker,
                    m.Groups["id"].Value,
                    m.Groups["version"]?.Value
                ))
                .Distinct(p => new { p.Id, p.Version })
                .OrderBy(p => p.Id)
                .ThenBy(p => p.Version)
                .ToList();
        }
    }
}
