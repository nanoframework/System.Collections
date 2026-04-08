# Copilot Instructions for nanoFramework System.Collections

## Repository Overview

This repository implements the `System.Collections` namespace for [.NET nanoFramework](https://www.nanoframework.net/), a free and open-source implementation of the .NET platform targeting highly constrained embedded/IoT devices (microcontrollers). It provides `Hashtable`, `Queue`, `Stack`, `IDictionary`, `IDictionaryEnumerator`, and `DictionaryEntry` — collection types optimized for resource-constrained environments.

The NuGet package produced is `nanoFramework.System.Collections`.

---

## Project System and Tooling

- **Project file extension**: `.nfproj` (nanoFramework project, not standard `.csproj`)
- **Solution file**: `nanoFramework.System.Collections.sln`
- **Target framework**: nanoFramework v1.0 (declared as `<TargetFrameworkVersion>v1.0</TargetFrameworkVersion>`)
- **Build tooling**: MSBuild with the nanoFramework Project System extension (Visual Studio on Windows only)
- **NuGet**: Managed via `packages.config` and `packages.lock.json` (old-style NuGet, not PackageReference). Lock files are enforced in CI (`<RestoreLockedMode>true</RestoreLockedMode>`).
- **Versioning**: [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) via `version.json`
- **Assembly signing**: Strong-named with `key.snk`

> **Important**: The nanoFramework build system requires Visual Studio with the nanoFramework VS extension installed. Builds **cannot** be done with a standard `dotnet build` command. In the Copilot cloud agent environment, builds are not possible. Validate changes through code review rather than build execution.

---

## Repository Structure

```
nanoFramework.System.Collections/      # Main library project
  Collections/
    Hashtable.cs                        # Hashtable implementation
    Hashtable.Bucket.cs                 # Internal Bucket class for Hashtable
    Queue.cs                            # Queue implementation
    Stack.cs                            # Stack implementation
    IDictionary.cs                      # IDictionary interface
    IDictionaryEnumerator.cs            # IDictionaryEnumerator interface
    DictionaryEntry.cs                  # Key/value pair class
  Properties/
    AssemblyInfo.cs                     # Assembly metadata (includes AssemblyNativeVersion)
  nanoFramework.System.Collections.nfproj
  key.snk                               # Strong-name key
  packages.config / packages.lock.json

Tests/
  HashtableTests/                       # Unit tests for Hashtable
  QueueTests/                           # Unit tests for Queue (includes TestObjects.cs)
  StackTests/                           # Unit tests for Stack

.github/
  workflows/
    pr-checks.yml                       # GitHub Actions: package lock and NuGet version checks
    update-dependencies.yml             # Automated dependency updates
    update-dependencies-develop.yml     # Automated dependency updates for develop
    generate-changelog.yml              # Changelog generation
  .changelog-config.json                # Changelog configuration

azure-pipelines.yml                     # Main CI/CD pipeline (Azure DevOps)
nanoFramework.System.Collections.nuspec # NuGet package specification
.runsettings                            # Test runner configuration
version.json                            # Nerdbank.GitVersioning config
NuGet.Config                            # NuGet source configuration
```

---

## Key Implementation Details

### Native Methods (InternalCall)

Several methods in `Queue` and `Stack` are implemented natively in the nanoCLR runtime (written in C++), **not** in C#. They are declared with `[MethodImpl(MethodImplOptions.InternalCall)]` and the `extern` keyword:

- **Queue**: `Clear()`, `CopyTo()`, `Enqueue()`, `Dequeue()`, `Peek()`
- **Stack**: `Clear()`, `Peek()`, `Pop()`, `Push()`

These methods **cannot be implemented or modified in C#**. Changes to their signatures require matching changes in the nanoCLR native runtime (separate repository). Do not remove `#pragma warning disable S4200` suppressions around these methods — Sonar warns about unwrapped native methods, and these are intentionally suppressed with explanatory comments.

### Hashtable Limitations vs. Standard .NET

The nanoFramework `Hashtable` differs significantly from .NET's:
- **No collision support**: Every key must produce a unique hash code via `GetHashCode()`. If two keys produce the same hash, behaviour is undefined.
- Uses `object.Equals()` for key comparisons (not a custom `IEqualityComparer`).
- Default load factor is `1.0f` and initial size is `3`.
- Uses a static `_syncLock` object for thread safety.

### Conditional Compilation

- `#if NANOCLR_REFLECTION` — wraps `[DebuggerDisplay(...)]` attributes on `Queue` and `Stack`. This attribute is not available in all nanoFramework builds (reflection may be disabled to save memory).

### Excluded Classes from Native PE

In the `.nfproj`, `DictionaryEntry`, `IDictionaryEnumerator`, and `ThisAssembly` are excluded from the native PE (via `NFMDP_PE_ExcludeClassByName`). These are pure managed types and don't need native representations.

### Namespace

All classes are in the `System.Collections` namespace, mirroring the standard .NET API surface for source compatibility.

---

## Testing

### Test Framework

Tests use `nanoFramework.TestFramework` (not xUnit/NUnit/MSTest). Key attributes:
- `[TestClass]` — marks a test class
- `[TestMethod]` — marks a test method
- `[Setup]` — marks a setup method (runs before tests)
- `Assert.*` — assertion methods (e.g., `Assert.AreEqual`, `Assert.IsTrue`, `Assert.IsNotNull`)

### Test Projects

Each collection has a dedicated test project under `Tests/`:
- `Tests/HashtableTests/HashtableTests.nfproj`
- `Tests/QueueTests/QueueTests.nfproj`
- `Tests/StackTests/StackTests.nfproj`

Test projects also use `.nfproj` and reference `nanoFramework.TestFramework` via `packages.config`.

### Running Tests

Tests run on the nanoCLR virtual machine (simulated hardware), configured via `.runsettings`:
- `IsRealHardware=False` — uses the nanoCLR simulator, not physical device
- `MaxCpuCount=1`, `TestSessionTimeout=120000ms`

In CI, tests are executed by the Azure Pipelines template with `runUnitTests: true` and `unitTestRunsettings: '$(System.DefaultWorkingDirectory)\.runsettings'`. Tests **cannot** be run in the Copilot cloud agent environment without the nanoFramework toolchain.

---

## CI/CD

### Azure Pipelines (Primary)

`azure-pipelines.yml` defines three jobs:
1. **Build_Library** — builds the solution and runs unit tests (on `windows-latest`)
2. **Update_Dependents** — triggered on tags or `***UPDATE_DEPENDENTS***` commit message; updates downstream repos
3. **Report_Build_Failure** — reports failures to Discord via webhook

The build uses templates from `nanoframework/nf-tools` repository. The SonarCloud project key is `nanoframework_lib-nanoFramework.System.Collections`.

### GitHub Actions

- **pr-checks.yml**: Runs on every PR; checks `packages.lock.json` consistency and verifies NuGet packages are up to date
- **update-dependencies.yml** / **update-dependencies-develop.yml**: Automated NuGet dependency bump PRs
- **generate-changelog.yml**: Auto-generates `CHANGELOG.md` from PR/commit history

---

## Conventions and Style

- **Copyright header**: Every `.cs` file must begin with:
  ```csharp
  //
  // Copyright (c) .NET Foundation and Contributors
  // Portions Copyright (c) Microsoft Corporation.  All rights reserved.
  // See LICENSE file in the project root for full license information.
  //
  ```
- **XML documentation**: All public types and members have `<summary>`, `<param>`, `<returns>`, and `<remarks>` XML doc comments.
- **Pragmas**: SonarLint suppression pragmas (`#pragma warning disable/restore S4200`) are used where needed with explanatory comments. Do not remove them.
- **`virtual` methods**: Most public methods are declared `virtual` following the original .NET Framework pattern.
- **`DictionaryEntry` fields**: `Key` and `Value` are public fields (not properties) to match the .NET Framework API — suppression `#pragma warning disable S1104` is intentional.
- **No generic collections**: This library provides only non-generic collections. Do not add generic (`List<T>`, `Dictionary<TKey, TValue>`, etc.) types — those belong in other nanoFramework packages.

---

## Making Changes

### Adding a New Collection Class

1. Create the `.cs` file under `nanoFramework.System.Collections/Collections/`
2. Add it to the `.nfproj` file (`<Compile Include="Collections\YourClass.cs" />`)
3. If it has native methods, coordinate with the nanoCLR native runtime repository
4. Create a corresponding test project under `Tests/YourClassTests/` with a `.nfproj`
5. Add the test project to `nanoFramework.System.Collections.sln`
6. Update `CHANGELOG.md` (or let the automated workflow handle it)

### Modifying Native Method Signatures

Changing the signature of a method marked `[MethodImpl(MethodImplOptions.InternalCall)]` requires:
1. Updating the C# declaration in this repo
2. Updating the native C++ implementation in the nanoCLR runtime (separate repo)
3. Bumping `AssemblyNativeVersion` in `Properties/AssemblyInfo.cs`

### Updating NuGet Dependencies

- Update `packages.config` with the new version
- Run `nuget restore` (or let the automated workflow create a PR)
- Regenerate `packages.lock.json` (run restore in locked mode locally, or let CI catch it)
- The `pr-checks.yml` workflow enforces that lock files are up to date

### AssemblyNativeVersion

The comment in `AssemblyInfo.cs` says "update this whenever the native assembly signature changes". This version (`AssemblyNativeVersion`) must match what the native runtime expects. Changing it without a corresponding native runtime change will break firmware compatibility.

---

## Known Errors and Workarounds

- **Cannot build in cloud agent**: The nanoFramework project system requires Visual Studio with the nF VS extension. `dotnet build` fails on `.nfproj` files. Workaround: validate changes through code reading and PR review; rely on Azure Pipelines CI for actual build validation.
- **Lock file errors in CI**: If `packages.lock.json` is out of sync, CI will fail the `check_package_lock` job. Regenerate the lock file locally with `nuget restore --force-evaluate` in the project directory, then commit the updated lock file.
- **SonarLint S4200 warnings**: Native (`extern`) methods trigger SonarLint's "Native methods should be wrapped" rule. These are suppressed with `#pragma warning disable/restore S4200` — do not remove these suppressions.
