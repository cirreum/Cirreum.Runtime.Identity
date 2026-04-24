# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is **Cirreum.Runtime.Identity** — the umbrella Runtime Extensions package for the Cirreum Identity provider family. It composes the per-protocol packages (`Cirreum.Runtime.Identity.Oidc` and `Cirreum.Runtime.Identity.EntraExternalId`) behind a single `AddIdentity()` / `MapIdentity()` pair. Install this one package when the application needs multiple identity-provider protocols; prefer a per-protocol package when a single protocol is used.

## Build Commands

```bash
dotnet build Cirreum.Runtime.Identity.slnx
dotnet pack --configuration Release
```

## Architecture

### What this package does

1. **`AddIdentity(builder, configure?)`** (`Extensions/Hosting/HostApplicationBuilderExtensions.cs`)
   - Calls `builder.AddOidcIdentity()` (no callback passed).
   - Calls `builder.AddEntraExternalIdIdentity()` (no callback passed).
   - Invokes the optional `Action<IIdentityBuilder>` callback exactly once against a single `IdentityBuilder(builder)` instance.
   - Rationale for not forwarding the callback to each per-protocol method: the callback typically registers keyed `IUserProvisioner` services via `AddProvisioner<T>(key)`, which is idempotent per key. Forwarding would cause each `AddProvisioner<T>(k)` to run twice (once inside each per-protocol `AddXxx`) — harmless (last-one-wins on resolution) but wasteful.

2. **`MapIdentity(endpoints)`** (`Extensions/Builder/EndpointRouteBuilderExtensions.cs`)
   - Resolves `IEnumerable<IdentityProviderMapping>` from DI.
   - Invokes every mapping's deferred `Map(endpoints)` closure — unfiltered (the per-protocol packages filter by `ProviderName`, the umbrella maps all).

### What this package does NOT do

- **Does not re-implement any Add/Map logic** — all behavior comes transitively from `Cirreum.Runtime.Identity.Oidc` and `Cirreum.Runtime.Identity.EntraExternalId`.
- **Does not register any provider directly** — it only composes the per-protocol packages that do.

## Project Structure

```
src/Cirreum.Runtime.Identity/
├── Extensions/
│   ├── Hosting/
│   │   └── HostApplicationBuilderExtensions.cs   # AddIdentity (umbrella)
│   └── Builder/
│       └── EndpointRouteBuilderExtensions.cs     # MapIdentity (umbrella)
└── Cirreum.Runtime.Identity.csproj
```

`RootNamespace` = `Cirreum.Runtime`, with extension classes in `Microsoft.Extensions.Hosting` / `Microsoft.AspNetCore.Builder` for discoverability.

## Dependencies

- **Cirreum.Runtime.Identity.Oidc** (brings `Cirreum.Identity.Oidc` + `Cirreum.Runtime.IdentityProvider` transitively)
- **Cirreum.Runtime.Identity.EntraExternalId** (brings `Cirreum.Identity.EntraExternalId` transitively)
- **Microsoft.AspNetCore.App**

## When to use this package vs. a per-protocol sibling

- **Single protocol:** install `Cirreum.Runtime.Identity.Oidc` OR `Cirreum.Runtime.Identity.EntraExternalId` directly. The binary only carries that protocol's infra code.
- **Multiple protocols:** install this umbrella. Both protocols' infra flows in transitively.
- **Never install the umbrella alongside a per-protocol package** — the umbrella already depends on both, so doing so is redundant and may surface duplicate extension-method definitions at compile time.

## Development Notes

- Uses .NET 10.0 with latest C# language version
- Nullable reference types enabled
- Extremely thin — two methods, both composition-only
- File-scoped namespaces
- K&R braces, tabs for indentation (matches repo `.editorconfig`)
