# Cirreum.Runtime.Identity v1.1.0

## Self-contained umbrella composition

`Cirreum.Runtime.Identity` no longer depends on its per-protocol sibling packages
(`Cirreum.Runtime.Identity.Oidc`, `Cirreum.Runtime.Identity.EntraExternalId`).
`AddIdentity()` now registers both provider protocols directly against the identity
runtime (`Cirreum.Runtime.IdentityProvider`) and the protocol packages
(`Cirreum.Identity.Oidc`, `Cirreum.Identity.EntraExternalId`).

### Why

A dependency on a package released in the same batch resolves one release behind:
when the umbrella re-pins before its siblings publish, NuGet lowest-wins resolution
hands umbrella-only consumers a stale transitive graph. v1.0.6 shipped exactly that
way — it resolved `Cirreum.Runtime.IdentityProvider` 1.0.6 instead of 1.1.0, silently
missing the orphaned-provisioner diagnostics that 1.1.0 delivers. Composing from the
layers below makes this class of gap structurally impossible.

### Impact

- **`AddIdentity()` / `MapIdentity()` are unchanged** — no code or configuration
  changes for apps using the umbrella's own surface.
- The per-protocol extension methods (`AddOidcIdentity()`, `MapOidcIdentity()`,
  `AddEntraExternalIdIdentity()`, `MapEntraExternalIdIdentity()`) no longer flow in
  transitively. Apps that called them while referencing only the umbrella should
  install the matching per-protocol package — or use the umbrella's own verbs, which
  cover both protocols.
- Single-protocol apps are unaffected: `Cirreum.Runtime.Identity.Oidc` and
  `Cirreum.Runtime.Identity.EntraExternalId` remain the right choice when only one
  protocol is needed, and are unchanged in this release.
