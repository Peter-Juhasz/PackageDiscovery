using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PackageDiscovery
{
    public class Program
    {
        internal const string DefaultOutputFileName = "output.tsv";
        internal const string DefaultFieldSeparator = "\t";

        public static void Main(string[] args)
        {
            // process arguments
            IReadOnlyCollection<DirectoryInfo> directories = (args ?? Enumerable.Empty<string>())
                .Where(a => !a.StartsWith("--"))
                .DefaultIfEmpty(Directory.GetCurrentDirectory())
                .Select(p => new DirectoryInfo(p))
                .ToList();

            // find
            IReadOnlyCollection<Package> packages = args.Contains("--installed")
                // find installed packages
                ? FindInstalledPackages(directories)
                // find referenced packages
                : FindReferencedPackages(directories);
            
            // render
            RenderResults(packages);
        }

        private static IReadOnlyCollection<Package> FindReferencedPackages(IReadOnlyCollection<DirectoryInfo> directories)
        {
            // initialize
            IReadOnlyCollection<IReferencedPackageFinder> packageFinders = typeof(IReferencedPackageFinder).GetTypeInfo().Assembly.GetExportedTypes()
                .Where(t => t.GetTypeInfo().GetCustomAttributes<ExportAttribute>().Any(a => a.ContractType == typeof(IReferencedPackageFinder)))
                .Select(Activator.CreateInstance)
                .Cast<IReferencedPackageFinder>()
                .ToList();
            
            // find packages
            return (
                from finder in packageFinders
                from directory in directories
                from package in finder.FindReferencedPackages(directory)
                select package
            ).ToList();
        }

        private static IReadOnlyCollection<Package> FindInstalledPackages(IReadOnlyCollection<DirectoryInfo> directories)
        {
            // initialize
            IReadOnlyCollection<IInstalledPackageFinder> packageFinders = typeof(IInstalledPackageFinder).GetTypeInfo().Assembly.GetExportedTypes()
                .Where(t => t.GetTypeInfo().GetCustomAttributes<ExportAttribute>().Any(a => a.ContractType == typeof(IInstalledPackageFinder)))
                .Select(Activator.CreateInstance)
                .Cast<IInstalledPackageFinder>()
                .ToList();
            
            // find packages
            return (
                from finder in packageFinders
                from directory in directories
                from package in finder.FindInstalledPackages(directory)
                select package
            ).ToList();
        }

        private static void RenderResults(IReadOnlyCollection<Package> packages)
        {
            packages = packages
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
