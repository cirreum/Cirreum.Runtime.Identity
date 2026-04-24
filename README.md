# Cirreum Runtime Identity

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Runtime.Identity.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.Identity/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Runtime.Identity.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.Identity/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Runtime.Identity?style=flat-square&labelColor=1F1F1F&color=FF3B2E)](https://github.com/cirreum/Cirreum.Runtime.Identity/releases)
[![License](https://img.shields.io/github/license/cirreum/Cirreum.Runtime.Identity?style=flat-square&labelColor=1F1F1F&color=F2F2F2)](https://github.com/cirreum/Cirreum.Runtime.Identity/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

**Umbrella Runtime Extensions package for the Cirreum Identity provider family. Install this one package to wire up every supported identity provider (OIDC webhook + Entra External ID) behind a single `AddIdentity()` / `MapIdentity()` pair.**

## Overview

Install `Cirreum.Runtime.Identity` when your application uses **more than one** identity-provider protocol — for example an API serving both Descope (OIDC webhook) clients and an Entra External ID tenant. Installing a single per-protocol package is preferable if you only need one of them (pay-for-what-you-use):

| Your app has... | Install |
|---|---|
| One OIDC-based IdP (Descope / Auth0 / …) | `Cirreum.Runtime.Identity.Oidc` |
| Entra External ID only | `Cirreum.Runtime.Identity.EntraExternalId` |
| Both (N+1 clients using different IdPs) | `Cirreum.Runtime.Identity` (this package) |

## Installation

```
dotnet add package Cirreum.Runtime.Identity
```

## Usage

```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.AddIdentity(p => p
    .AddProvisioner<ClientABorrowerProvisioner>("clientA_descope")    // Oidc instance
    .AddProvisioner<ClientBBorrowerProvisioner>("clientB_descope")    // Oidc instance
    .AddProvisioner<EmployeeProvisioner>("primary"));                 // EntraExternalId instance

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentity();

app.Run();
```

`AddIdentity()` registers both the Oidc and EntraExternalId providers from their respective `Cirreum:Identity:Providers:*` configuration sections. `MapIdentity()` maps every enabled instance's route across both providers.

App-provided provisioner classes derive from the base that matches each instance's onboarding model (`InvitationUserProvisionerBase<TUser>` or `SelfServiceUserProvisionerBase<TUser>`). See `Cirreum.IdentityProvider` for the provisioner hierarchy and the sibling packages for each protocol's wire contract and configuration:

- [`Cirreum.Identity.Oidc`](https://www.nuget.org/packages/Cirreum.Identity.Oidc/) — OIDC webhook configuration, wire contract, security model
- [`Cirreum.Identity.EntraExternalId`](https://www.nuget.org/packages/Cirreum.Identity.EntraExternalId/) — Entra External ID setup, tenant-ID issuer format, Azure Portal walkthrough

## What this package contains

Just two extension methods composing the per-protocol packages:

- **`builder.AddIdentity(configure?)`** — calls `AddOidcIdentity()` and `AddEntraExternalIdIdentity()` (each dedups its own provider registration via marker type), then invokes the configure callback once against a single `IdentityBuilder` so `AddProvisioner<T>(key)` calls run once per key.
- **`app.MapIdentity()`** — invokes every registered `IdentityProviderMapping` (unfiltered). Per-protocol packages still expose their own filtered `MapOidcIdentity()` / `MapEntraExternalIdIdentity()` if you want granular control.

## Dependencies

- **Cirreum.Runtime.Identity.Oidc** (brings in `Cirreum.Identity.Oidc` + `Cirreum.Runtime.IdentityProvider` transitively)
- **Cirreum.Runtime.Identity.EntraExternalId** (brings in `Cirreum.Identity.EntraExternalId` transitively)

## License

MIT — see [LICENSE](LICENSE).

---

**Cirreum Foundation Framework**  
*Layered simplicity for modern .NET*
