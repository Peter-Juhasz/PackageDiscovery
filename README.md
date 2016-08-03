# Package Discovery
Find all referenced or installed packages in your solutions for:
 - Build reporting
 - Find version mismatches across multiple projects
 - Security reporting

## Example report
```
Bower   angular             1.5.7
Bower   angular-route       1.5.7
NPM     gulp                *
NuGet   System.Interactive  3.0.0
```
## Supported package managers

Package Manager | Referenced | Installed
--------------- | ---------- | ---------
Bower           | X          | 
CocoaPods       | X          |
Composer        | X          |
Dockerfile      | X          |
NPM             | X          | X
NuGet           | X          |
TSD             | X          |
Typings         | X          |

## Usage
Written using **.NET Core 1.0.0**, runs everywhere.

Windows:
`PackageDiscovery [paths to search]`

Linux:
`dotnet PackageDiscovery [paths to search]`

### Arguments
Argument       | Description
-------------- | -----------
--installed    | Find installed packages
