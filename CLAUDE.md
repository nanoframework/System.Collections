# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository overview

This is the **nanoFramework.System.Collections** library — a re-implementation of the `System.Collections` / `System.Collections.Generic` collection types for [.NET **nanoFramework**](https://nanoframework.net/), a stripped-down .NET runtime that runs directly on embedded microcontrollers (ESP32, STM32, etc.). It is NOT a normal .NET/.NET Standard project: it targets a custom, constrained CLR (`nanoCLR`) with its own project system, MSBuild tooling, and a `mscorlib` that only implements a subset of the BCL.

Key implication for any change: don't assume APIs from desktop `System.Collections` are available (LINQ, `Span<T>`, `IEqualityComparer<T>` generics customization, reflection-heavy patterns, etc. may not exist in nanoFramework's `mscorlib`). Always check `nanoFramework.System.Collections/Collections/**` for existing patterns before introducing a new API surface.

## Solution layout

- `nanoFramework.System.Collections.sln` — main solution.
- `nanoFramework.System.Collections/` — the library project (`nanoFramework.System.Collections.nfproj`, `RootNamespace=System.Collections`, `NF_IsCoreLibrary=True`).
  - `Collections/` — non-generic types: `Hashtable` (+ `Hashtable.Bucket.cs`), `Queue`, `Stack`, `IDictionary`, `IDictionaryEnumerator`, `DictionaryEntry`.
  - `Collections/Generic/` — generic types: `Dictionary<TKey,TValue>`, `List<T>`, `KeyValuePair<TKey,TValue>`, `IDictionary<TKey,TValue>`, `IList<T>`, `IReadOnlyCollection<T>`, `IReadOnlyDictionary<TKey,TValue>`, `IReadOnlyList<T>`, debugger-view helpers.
- `Tests/` — one `.nfproj` test project per area, each producing an `NFUnitTest` assembly and referencing the library via `ProjectReference`:
  - `HashtableTests/`, `QueueTests/`, `StackTests/`, `GenericCollections/` (covers `Dictionary<TKey,TValue>` and `List<T>`).
  - Tests use `nanoFramework.TestFramework` (`[TestClass]`, `[TestMethod]`, `[Setup]`, `Assert.*` — MSTest-like API, not xUnit/NUnit).
- `packages/` — restored NuGet packages, including the pinned nanoCLR/test-runner tooling (checked into `packages.lock.json` per project).
- `version.json` — Nerdbank.GitVersioning config; version is derived from git height, do not hand-edit assembly versions.
- `azure-pipelines.yml` — CI entry point (Azure DevOps, using shared templates from `nanoframework/nf-tools`). Runs build, unit tests (via `.runsettings`), SonarCloud analysis, and NuGet publish.
- `.github/workflows/pr-checks.yml` — GitHub Actions PR checks: verifies `packages.lock.json` is current and that referenced nanoFramework NuGet packages are up to date.

## Prerequisites for local development

Building/testing requires Visual Studio 2022 with the **nanoFramework VS2022 extension** installed (provides the `nanoFramework` MSBuild project system imported by every `.nfproj`, i.e. `$(MSBuildExtensionsPath)\nanoFramework\v1.0\NFProjectSystem.*`). Without the extension, `.nfproj` files will not build from the command line via plain `dotnet build`/`msbuild`.

Building/running tests also requires the **nanoFramework Test Explorer / nanoclr runner** components (installed alongside the VS extension, or via `nanoclr` global tool) since tests execute against the nanoCLR interpreter, not the desktop CLR.

## Common commands

```bash
# Restore packages for the whole solution (locked mode, matches CI)
nuget restore nanoFramework.System.Collections.sln

# Build the full solution in Visual Studio / via msbuild (requires nanoFramework VS extension)
msbuild nanoFramework.System.Collections.sln /p:Configuration=Release /p:Platform="Any CPU"
```

Running tests locally is normally done through Visual Studio's Test Explorer (the nanoFramework Test Adapter executes each `.nfproj` test assembly against a virtual nanoCLR instance). Solution-wide settings for the test run live in [.runsettings](.runsettings); each test project also has its own `nano.runsettings`. There is no plain `dotnet test` path — the adapter is VS/nanoclr-specific.

To run a **single test**, use Visual Studio Test Explorer and run/debug the individual `[TestMethod]`. There is no supported CLI equivalent in this repo.

## Architecture notes

- **`Hashtable`** (`Collections/Hashtable.cs` + `Hashtable.Bucket.cs`) is a from-scratch, non-generic hashtable. Unlike desktop .NET's `Hashtable`, **it does not support hash collisions** — every key must produce a truly unique `GetHashCode()` within the table (see the type's XML doc remarks). Backed by a private `Bucket[]` array (`internal class Bucket { object _key; object _value; uint _hash; }`), sized via `InitialSize`/load factor, with a static `_syncLock` for thread safety and a `_version` field for enumerator invalidation (fail-fast on concurrent modification).
- **`Dictionary<TKey, TValue>`** (`Collections/Generic/Dictionary.cs`) mirrors the modern desktop BCL implementation's bucket/entries-with-free-list design (`_buckets`, `_entries`, `StartOfFreeList` encoding for the free chain, `_freeList`/`_freeCount`), including `KeyCollection`/`ValueCollection` nested views. Keys are constrained `where TKey : notnull`. This is a different, newer implementation strategy than `Hashtable` — don't assume the two share internals.
- **`List<T>`**, `Queue`, `Stack` follow standard growable-array patterns.
- Non-generic and generic collection interfaces are hand-rolled locally (`Collections/IDictionary.cs`, `Collections/Generic/IList.cs`, etc.) rather than coming from a full BCL — check whether an interface member you expect actually exists here before using it.
- The `.nfproj` excludes certain classes from stub/skeleton generation for the native interop layer (`NFMDP_PE_ExcludeClassByName` in the `.nfproj`: `ThisAssembly`, `DictionaryEntry`, `IDictionaryEnumerator`) — these are pure-managed types with no native counterpart.
- Assembly is strong-named (`key.snk`, `SignAssembly=true`) and versioned via Nerdbank.GitVersioning (`version.json`) — don't hand-edit version/assembly-info files.

## CI/PR expectations

- `packages.lock.json` files (one per `.nfproj`) must stay in sync with `packages.config`; PR checks fail if they drift. Restore with locked mode (`RestoreLockedMode=true` is forced when `TF_BUILD`/`ContinuousIntegrationBuild` is set) rather than hand-editing lock files.
- CI also checks that referenced nanoFramework NuGet packages (`nanoFramework.CoreLibrary`, `nanoFramework.TestFramework`, etc.) are current — expect automated dependency-update PRs/commits (see recent git history) targeting `.nfproj`/`packages.config`/`packages.lock.json`.
