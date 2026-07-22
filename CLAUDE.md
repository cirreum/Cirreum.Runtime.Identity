# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is **Cirreum.Runtime.Identity** — the umbrella Runtime Extensions package for the Cirreum Identity provider family. It registers every shipped provider protocol (`Oidc` and `EntraExternalId`) behind a single `AddIdentity()` / `MapIdentity()` pair, composing directly against the identity runtime (`Cirreum.Runtime.IdentityProvider`) and the protocol packages (`Cirreum.Identity.Oidc`, `Cirreum.Identity.EntraExternalId`). Install this one package when the application needs multiple identity-provider protocols; prefer a per-protocol package when a single protocol is used.

**No same-layer dependencies.** The umbrella deliberately does NOT reference its per-protocol siblings (`Cirreum.Runtime.Identity.Oidc`, `Cirreum.Runtime.Identity.EntraExternalId`) even though they wrap the same registrations — an intra-layer dependency ships one release behind under batch release + lowest-wins resolution, handing umbrella-only consumers a stale transitive graph. The small registration duplication is the deliberate price for a structurally correct dependency floor.

## Build Commands

```bash
dotnet build Cirreum.Runtime.Identity.slnx
dotnet pack --configuration Release
```

## Architecture

### What this package does

1. **`AddIdentity(builder, configure?)`** (`Extensions/Hosting/HostApplicationBuilderExtensions.cs`)
   - Marker-type dedup via `AddIdentityMarker` — provider registration runs once even across repeat calls.
   - Calls `builder.RegisterIdentityProvider<TRegistrar, TSettings, TInstanceSettings>()` (from `Cirreum.Runtime.IdentityProvider`) once per shipped protocol: Oidc and EntraExternalId.
   - Invokes the optional `Action<IIdentityBuilder>` callback exactly once against a single `IdentityBuilder(builder)` instance, so each `AddProvisioner<T>(key)` runs once per key (not per protocol × key).

2. **`MapIdentity(endpoints)`** (`Extensions/Builder/EndpointRouteBuilderExtensions.cs`)
   - Resolves `IEnumerable<IdentityProviderMapping>` from DI.
   - Invokes every mapping's deferred `Map(endpoints)` closure — unfiltered (the per-protocol packages filter by `ProviderName`, the umbrella maps all).

### What this package does NOT do

- **Does not re-implement any registrar, handler, or settings type** — those live in the protocol packages (`Cirreum.Identity.Oidc`, `Cirreum.Identity.EntraExternalId`); the registration plumbing lives in `Cirreum.Runtime.IdentityProvider`.
- **Does not register `IUserProvisioner`** — that's the app's job, via the `IIdentityBuilder.AddProvisioner<T>(key)` callback.
- **Does not expose per-protocol verbs** — `AddOidcIdentity()` etc. live in the per-protocol packages only.

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

- **Cirreum.Runtime.IdentityProvider** — `RegisterIdentityProvider<>` helper, `IIdentityBuilder` + `IdentityBuilder`, `IdentityProviderMapping`
- **Cirreum.Identity.Oidc** — Oidc registrar + settings types (the `RegisterIdentityProvider<>` generic arguments)
- **Cirreum.Identity.EntraExternalId** — Entra External ID registrar + settings types
- **Microsoft.AspNetCore.App**

## When to use this package vs. a per-protocol sibling

- **Single protocol:** install `Cirreum.Runtime.Identity.Oidc` OR `Cirreum.Runtime.Identity.EntraExternalId` directly. The binary only carries that protocol's infra code.
- **Multiple protocols:** install this umbrella. Both protocols are registered by `AddIdentity()`.
- **Never install the umbrella alongside a per-protocol package** — both would register the same protocol independently (the umbrella and the per-protocol package use separate dedup markers), and both sets of Add/Map verbs would be in scope.

## Development Notes

- Uses .NET 10.0 with latest C# language version
- Nullable reference types enabled
- Extremely thin — two methods, both composition-only
- File-scoped namespaces
- K&R braces, tabs for indentation (matches repo `.editorconfig`)
