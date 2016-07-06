using PackageDiscovery.Finders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PackageDiscovery
{
    public class Program
    {
        internal const string DefaultOutputFileName = "output.tsv";
        internal const string DefaultFieldSeparator = "\t";

        public static void Main(string[] args)
        {
            // initialize
            IReadOnlyCollection<IPackageFinder> packageFinders = new IPackageFinder[]
            {
                new BowerPackageFinder(),
                new ComposerPackageFinder(),
                new NPMPackageFinder(),
                new NuGetPackageFinder(),
            };

            // process arguments
            IReadOnlyCollection<DirectoryInfo> directories = (args ?? Enumerable.Empty<string>())
                .DefaultIfEmpty(Directory.GetCurrentDirectory())
                .Select(p => new DirectoryInfo(p))
                .ToList();

            // find packages
            IReadOnlyCollection<Package> packages = (
                from finder in packageFinders
                from directory in directories
                from package in finder.FindPackages(directory)
                select package
            )
                .Distinct(p => new { p.Kind, p.Id, p.Version, p.IsDevelopmentPackage })
                .OrderBy(p => p.Kind)
                    .ThenBy(p => p.Id)
                    .ThenBy(p => p.Version)
                    .ThenBy(p => p.IsDevelopmentPackage)
                .ToList();

            // write output
            if (File.Exists(DefaultOutputFileName))
                File.Delete(DefaultOutputFileName);

            using (TextWriter writer = File.CreateText(DefaultOutputFileName))
            {
                foreach (Package package in packages)
                {
                    string row = String.Join(DefaultFieldSeparator, new object[] { package.Kind, package.Id, package.Version, package.IsDevelopmentPackage });
                    writer.WriteLine(row);
                }
            }
        }
    }
}
