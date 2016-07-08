# Package Discovery
Find all referenced packages in your solutions for:
 - Build reporting
 - Find version mismatches across multiple projects
 - Security reporting

Written using **.NET Core 1.0.0**, runs everywhere.

Supported package managers:
 - Composer
 - Bower
 - NPM
 - NuGet
 - TSD
 - Typings

## Usage
Windows:
`PackageDiscovery [paths to search]`

Linux:
`dotnet PackageDiscovery [paths to search]`

The reported is generated in a new file named `output.tsv`:
```
Bower   angular             1.5.7
Bower   angular-route       1.5.7
NPM     gulp                *
NuGet   System.Interactive  3.0.0
```
