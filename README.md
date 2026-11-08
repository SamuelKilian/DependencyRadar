# Dependency Radar

**A lightweight NuGet vulnerability scanner built with C# and ASP.NET Core.**

Dependency Radar allows developers to check NuGet package versions against the [OSV vulnerability database](https://osv.dev/).

## Overview

Dependency Radar is a web application that scans NuGet dependencies for publicly known security vulnerabilities.

Users can enter package names and versions, run a scan, and view the detected vulnerabilities along with available fixed versions.

## Features

- Scan multiple NuGet packages in a single request.
- Query the OSV vulnerability database.
- Display detected vulnerabilities and their GHSA identifiers.
- Extract available fixed versions.
- Display scan results through a dark-themed web interface.
- Handle API errors and request timeouts.
- Unit tests for core services.

## Technologies

| Component | Technology |
|---|---|
| Backend | C# / ASP.NET Core |
| Frontend | HTML, CSS, JavaScript |
| Vulnerability database | OSV API |
| Testing | xUnit |
| Framework | .NET 10 |

## Project Structure

```text
DependencyRadar-Repository/
├── DependencyRadar/
│   ├── Models/
│   ├── Services/
│   ├── wwwroot/
│   ├── Program.cs
│   └── DependencyRadar.csproj
│
├── DependencyRadar.Tests/
│   ├── FixedVersionExtractorTests.cs
│   ├── OsvServiceTests.cs
│   └── DependencyRadar.Tests.csproj
│
└── README.md
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Internet connection to access the OSV API.

### Run the application

Clone the repository:

```bash
git clone YOUR_REPOSITORY_URL
cd DependencyRadar-Repository
```

Run the application:

```bash
dotnet run --project DependencyRadar/DependencyRadar.csproj
```

Open the local URL displayed in the terminal to access the application.

### Run the tests

```bash
dotnet test DependencyRadar.Tests/DependencyRadar.Tests.csproj
```

## Future Improvements

- Validate NuGet package existence before scanning.
- Improve vulnerability severity classification.
- Import dependencies directly from `.csproj` files.
- Add scan history.
- Support exporting scan reports.

## Limitations

A scan with no vulnerabilities returned by OSV does not guarantee that a package is free of security issues. It only means that no matching known vulnerabilities were returned by the database.

Fixed versions are extracted from OSV vulnerability data and should be reviewed before upgrading a dependency.

## License

To be determined.
