# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Updated

- **`Cirreum.Runtime.Identity.Oidc`** / **`Cirreum.Runtime.Identity.EntraExternalId`** —
  `1.0.5` → `1.0.6`. Corrects dependency floors that 1.0.6 shipped one release behind:
  under NuGet lowest-wins resolution, consumers referencing only this umbrella resolved
  the pre-wave identity graph (`Cirreum.Runtime.IdentityProvider` 1.0.6 instead of 1.1.0,
  `Cirreum.IdentityProvider` 1.0.7 instead of 1.0.8) and silently missed the
  orphaned-provisioner diagnostics and registration fixes those versions deliver.

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

