# Robotico.Repository.Mishima

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![C#](https://img.shields.io/badge/C%23-latest-239120?logo=csharp)](https://learn.microsoft.com/dotnet/csharp/)
[![MishimaDocs](https://img.shields.io/badge/MishimaDocs-engine-5C4EE5)](https://www.nuget.org/packages/MishimaDocs)
[![NuGet](https://img.shields.io/badge/NuGet-Robotico.Repository.Mishima-blue?logo=nuget)](https://github.com/robotico-dev/robotico-repository-mishimadocs-csharp/packages)
[![Robotico](https://img.shields.io/badge/Robotico-Tier%203%20adapter-1f883d?logo=github)](https://github.com/robotico-dev)

NuGet package id: **`Robotico.Repository.Mishima`** — `IRepository<TEntity, TId>` and `IAsyncRepository<TEntity, TId>` over the **`MishimaDocs`** engine (JSON documents per entity id). This repo slug uses `mishimadocs` for checkout layout only.

**Target framework:** `net10.0` only (matches the MishimaDocs engine).

## Which interface?

| Host / scenario | Prefer |
|-----------------|--------|
| Async hosts with `IMishimaAsyncDatabase` and `HasAsyncPersistence` | `IAsyncRepository<,>` for non-blocking writes (`IMishimaAsyncCollection`) |
| Sync-only opens or reads | `IRepository<,>`; `GetByIdAsync` uses synchronous MishimaDocs reads |

## Unit of work profile (`IUnitOfWorkCapabilities`)

| | |
|--|--|
| `UnitOfWorkCommitMode` | `NoOpCommitSuccess` |
| `CommitCoordinatesDomainWrites` | no — each repository call persists immediately |
| `SupportsTransactions` | no — use MishimaDocs `IMishimaWriteBatch` / `IMishimaAsyncWriteBatch` for atomic multi-document writes |

`MishimaUnitOfWork.CommitAsync` succeeds but does not batch repository operations; align expectations with `UnitOfWorkGuard` in apps that require deferred commits.

## Build (Robotico monorepo)

**MishimaDocs (dual mode):** with the monorepo layout (`csharp/robotico-repository-mishimadocs-csharp` and `mishima-suite` two levels above this folder), MSBuild binds the engine via **ProjectReference** (`MishimaDocsProjectPath`). Standalone clones without those sources consume **`MishimaDocs`** as a **PackageReference** (version in `Directory.Packages.props`). See `csharp/build/ROBOTICO_NUGET_DUAL_MODE_PLAN.adoc` and `ROBOTICO_REPOSITORY_OUTBOX_MISHIMA_DOCS_IMPLEMENTATION.adoc`.

```bash
dotnet restore Robotico.Repository.Mishima.sln
dotnet build Robotico.Repository.Mishima.sln -c Release
dotnet test Robotico.Repository.Mishima.sln -c Release
```

`MishimaUnitOfWork` is a no-op `CommitAsync` (each repository call commits immediately through MishimaDocs). Use `IMishimaWriteBatch` in application code when you need atomic multi-document writes.

## CI and quality bar

GitHub Actions runs build, tests, coverlet, a line-coverage gate on **Robotico.Repository.Mishima**, and — in the umbrella `csharp/` layout — `verify-robotico-library-bar.ps1`. Same rules as other Robotico C# libraries: one top-level type per file, explicit types (`.editorconfig` / `IDE0008`). See `csharp/build/ROBOTICO_LIBRARY_10_STANDARD.adoc`.

## License

See [LICENSE](LICENSE).
