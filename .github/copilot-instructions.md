# Copilot Instructions for nanoFramework.System.Collections

## Repository Overview

This is a **.NET nanoFramework** class library that provides collection types for use on microcontrollers. It is **not** a standard .NET library — it targets the nanoFramework runtime (nanoCLR), which runs on resource-constrained embedded devices. Code runs on hardware or a virtual nanoCLR device, not on the standard .NET CLR.

NuGet package name: `nanoFramework.System.Collections`

## Project Structure

```
nanoFramework.System.Collections/    # Main library project (.nfproj)
  Collections/
    Hashtable.cs                     # IDictionary implementation (no collision support)
    Hashtable.Bucket.cs              # Internal bucket type for Hashtable
    Queue.cs                         # Circular-array queue (some methods are native)
    Stack.cs                         # Array-backed stack (some methods are native)
    DictionaryEntry.cs               # Key/value pair struct
    IDictionary.cs                   # IDictionary interface
    IDictionaryEnumerator.cs         # IDictionaryEnumerator interface
    Generic/
      List.cs                        # Generic List<T> implementation
      IList.cs                       # IList<T> interface
      IReadOnlyCollection.cs
      IReadOnlyList.cs
Tests/
  HashtableTests/                    # Unit tests for Hashtable
  QueueTests/                        # Unit tests for Queue
  StackTests/                        # Unit tests for Stack
  GenericCollections/                # Unit tests for List<T> and generic interfaces
.github/
  workflows/
    pr-checks.yml                    # PR checks: package lock + NuGet version validation
    update-dependencies.yml          # Updates downstream dependent repos
    update-dependencies-develop.yml
azure-pipelines.yml                  # Main CI/CD pipeline (build, test, publish NuGet)
nanoFramework.System.Collections.sln # Visual Studio solution
nanoFramework.System.Collections.nuspec # NuGet package spec
version.json                         # Nerdbank.GitVersioning configuration
.runsettings                         # Test runner settings for nanoCLR
```

## Key Technical Details

### nanoFramework Project Format
- Project files use `.nfproj` extension (not `.csproj`). These are MSBuild projects targeting `TargetFrameworkVersion v1.0` with nanoFramework-specific tooling.
- The `ProjectTypeGuids` includes `{11A8DD76-328B-46DF-9F39-F559912D0360}` (nanoFramework) alongside the standard C# GUID.
- Requires the nanoFramework Visual Studio extension and MSBuild extensions installed at `$(MSBuildExtensionsPath)\nanoFramework\v1.0\`.
- **Build is Windows-only** — CI uses `vmImage: 'windows-latest'`. Local builds require Windows + Visual Studio with nanoFramework extension.

### Native Methods
Some collection methods are implemented in the nanoCLR firmware (C++ native code), not in C#. These are declared with `extern` and `[MethodImpl(MethodImplOptions.InternalCall)]`:
```csharp
[MethodImpl(MethodImplOptions.InternalCall)]
public virtual extern void Clear();
```
These methods cannot be modified in this repository — only the managed C# wrapper layer lives here. The `#pragma warning disable S4200` suppresses SonarQube warnings about unwrapped native methods.

### Conditional Compilation
Code uses `#if NANOCLR_REFLECTION` to conditionally include features (e.g., `[DebuggerDisplay]`) that depend on nanoFramework's reflection support being enabled.

### Hashtable Limitation
Unlike full .NET, the nanoFramework `Hashtable` does **not** support hash collisions. Every key must produce a truly unique hash code via `GetHashCode()`.

### Assembly Signing
The library is strong-named using `key.snk`. Do not remove or replace this file.

### Versioning
Versions are managed by **Nerdbank.GitVersioning** via `version.json`. Do not manually edit version numbers in project files or AssemblyInfo.

## Test Framework

Tests use the **nanoFramework.TestFramework** (not xUnit/NUnit/MSTest). Key attributes:
- `[TestClass]` — marks a test class
- `[TestMethod]` — marks a test method
- `[Setup]` — runs before tests in the class
- `Assert.AreEqual(expected, actual)`, `Assert.IsTrue(...)`, etc.

Tests run on the **nanoCLR virtual device** (not the host .NET runtime). The `.runsettings` file configures this:
```xml
<nanoFrameworkAdapter>
  <IsRealHardware>False</IsRealHardware>
  <UsePreviewClr>True</UsePreviewClr>
</nanoFrameworkAdapter>
```

Each test project has its own `.nfproj` and `nano.runsettings` file.

## Building and Testing

**Local build is only possible on Windows with Visual Studio** and the nanoFramework Visual Studio extension installed. There is no cross-platform CLI build support.

The CI pipeline (`azure-pipelines.yml`) uses a shared template from `nanoframework/nf-tools`:
```yaml
- template: azure-pipelines-templates/class-lib-build.yml@templates
  parameters:
    runUnitTests: true
    unitTestRunsettings: '$(System.DefaultWorkingDirectory)\.runsettings'
    usePreviewBuild: true
```

NuGet package restore uses locked mode in CI (`RestoreLockedMode=true` when `TF_BUILD=True`). If you update NuGet dependencies, you must update `packages.lock.json` files accordingly.

## GitHub Workflows

- **PR checks** (`pr-checks.yml`): Validates `packages.lock.json` is up to date and NuGet packages reference the latest stable versions.
- **update-dependencies**: Automatically updates downstream repositories (e.g., `System.Net.Http`, `nanoFramework.Json`) after a release.
- CI build failures are reported to Discord via webhook.

## Code Style Conventions

- Copyright header on every file:
  ```csharp
  // Copyright (c) .NET Foundation and Contributors
  // Portions Copyright (c) Microsoft Corporation.  All rights reserved.
  // See LICENSE file in the project root for full license information.
  ```
- XML doc comments (`/// <summary>`) on all public members.
- Namespaces: `System.Collections` and `System.Collections.Generic` (matching full .NET API surface).
- Use `var` for local variables where type is clear from context.
- SonarQube pragmas (`#pragma warning disable/restore`) used sparingly for known false positives.
- `#nullable enable` is used in `List.cs`.

## Packages and Dependencies

- Runtime dependency: `nanoFramework.CoreLibrary` (`mscorlib`)
- Build tooling: `Nerdbank.GitVersioning`
- Test projects reference `nanoFramework.TestFramework` and `nanoFramework.UnitTestLauncher`
- `NuGet.Config` configures the nanoFramework preview NuGet feed in addition to nuget.org

## Common Tasks and Known Gotchas

1. **Cannot run tests locally without Windows + Visual Studio + nanoFramework extension.** The test runner deploys to a virtual nanoCLR device.

2. **Do not use standard .NET APIs** that don't exist in nanoFramework's reduced `mscorlib`. The nanoFramework runtime is a strict subset of .NET — features like LINQ, `Span<T>`, reflection-heavy APIs, and many BCL types are unavailable or limited.

3. **Updating NuGet packages**: After adding or updating a NuGet package reference in a `.nfproj`, regenerate `packages.lock.json` by running NuGet restore. The PR check will fail if lock files are stale.

4. **Adding new collection types**: Follow the existing pattern — implement managed-layer C# in `nanoFramework.System.Collections/Collections/`, add the file to the `.nfproj` `<Compile>` list, and add corresponding tests in `Tests/`.

5. **Branch strategy**: `main` (stable releases), `develop` (preview builds), `release-*` (release branches). CI publishes NuGet packages on tag pushes (`v*`).

6. **SonarCloud** analysis runs as part of CI under project `nanoframework_lib-nanoFramework.System.Collections`.
