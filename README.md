# Robotico.Repository.Mishima

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![C#](https://img.shields.io/badge/C%23-latest-239120?logo=csharp)](https://learn.microsoft.com/dotnet/csharp/)
[![MishimaDocs](https://img.shields.io/badge/MishimaDocs-engine-5C4EE5)](https://www.nuget.org/packages/MishimaDocs)
[![NuGet](https://img.shields.io/badge/NuGet-Robotico.Repository.Mishima-blue?logo=nuget)](https://github.com/robotico-dev/robotico-repository-mishimadocs-csharp/packages)
[![Robotico](https://img.shields.io/badge/Robotico-Tier%203%20adapter-1f883d?logo=github)](https://github.com/robotico-dev)

NuGet package id: **`Robotico.Repository.Mishima`** — `IRepository<TEntity, TId>` over the **`MishimaDocs`** engine (JSON documents per entity id). This repo slug uses `mishimadocs` for checkout layout only.

**Target framework:** `net10.0` only (matches the MishimaDocs engine).

## Build (Robotico monorepo)

This checkout is expected under `csharp/robotico-repository-mishimadocs-csharp` next to `mishima-suite/mishima-docs` so the **ProjectReference** to `MishimaDocs` resolves.

```bash
dotnet restore Robotico.Repository.Mishima.sln
dotnet build Robotico.Repository.Mishima.sln -c Release
dotnet test Robotico.Repository.Mishima.sln -c Release
```

`MishimaUnitOfWork` is a no-op `CommitAsync` (each repository call commits immediately through MishimaDocs). Use `IMishimaWriteBatch` in application code when you need atomic multi-document writes.

## License

See [LICENSE](LICENSE).
