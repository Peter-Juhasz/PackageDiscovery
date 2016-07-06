# Package Discovery
Find all referenced packages in your solutions.

Written using **.NET Core 1.0.0**, runs everywhere.

Supported package managers:
 - Bower
 - NPM
 - NuGet

## Use cases
 - Security reporting
 - Find version mismatches across multiple projects

## Usage
`dotnet PackageDiscovery [paths to search]`

The reported is generated into a file named `output.tsv`:
```
Bower   angular             1.5.7
NPM     gulp                *
NuGet   System.Interactive  3.0.0
```
