# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Updated

- Updated NuGet packages.

## [1.1.2] - 2026-07-24

### Updated

- Updated NuGet packages.

## [1.1.1] - 2026-07-23

### Updated

- Updated NuGet packages.

## [1.1.0] - 2026-07-22

### Changed

- **Self-contained composition — same-layer sibling dependencies removed.** The umbrella
  no longer references `Cirreum.Runtime.Identity.Oidc` / `Cirreum.Runtime.Identity.EntraExternalId`;
  `AddIdentity()` now registers both provider protocols directly against
  `Cirreum.Runtime.IdentityProvider` (1.1.0) and the protocol packages
  (`Cirreum.Identity.Oidc` 1.0.7, `Cirreum.Identity.EntraExternalId` 2.0.9). A same-layer
  dependency ships one release behind under batch release + lowest-wins resolution — 1.0.6
  resolved the pre-wave identity graph (`Cirreum.Runtime.IdentityProvider` 1.0.6 instead
  of 1.1.0) for umbrella-only consumers; composing from the layers below makes that gap
  structurally impossible. The umbrella's own surface (`AddIdentity()` / `MapIdentity()`)
  is unchanged. Apps that relied on the umbrella to transitively supply the per-protocol
  verbs (`AddOidcIdentity()`, `MapOidcIdentity()`, `AddEntraExternalIdIdentity()`,
  `MapEntraExternalIdIdentity()`) should install the matching per-protocol package —
  or, preferably, use the umbrella's own verbs.

## [1.0.6] - 2026-07-20

### Updated

- Updated NuGet packages.

## [1.0.5] - 2026-07-09

### Updated

- Updated NuGet packages.

## [1.0.4] - 2026-05-10

### Updated

- Updated NuGet packages.

## [1.0.3] - 2026-05-10

### Updated

- Updated NuGet packages.

## [1.0.2] - 2026-05-01

### Updated
- Updated NuGet packages.

