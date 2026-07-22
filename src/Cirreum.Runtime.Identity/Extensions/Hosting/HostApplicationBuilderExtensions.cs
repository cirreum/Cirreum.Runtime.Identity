namespace Microsoft.Extensions.Hosting;

using Cirreum.Identity;
using Cirreum.Identity.Configuration;
using Cirreum.Identity.Provisioning;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// App-facing umbrella extensions for the Cirreum Identity provider family — registers
/// every shipped provider protocol (Oidc and EntraExternalId) behind a single
/// <c>AddIdentity()</c> entry point.
/// </summary>
public static class HostApplicationBuilderExtensions {

	private sealed class AddIdentityMarker { }

	/// <summary>
	/// Registers every Cirreum Identity provider shipped in the umbrella (Oidc and
	/// EntraExternalId). Enabled instances from configuration for each provider are wired
	/// up; the optional <paramref name="configure"/> callback lets the app register
	/// per-instance <see cref="IUserProvisioner"/> implementations once, across both
	/// providers.
	/// </summary>
	/// <param name="builder">The host application builder.</param>
	/// <param name="configure">
	/// Optional callback to register per-instance <see cref="IUserProvisioner"/>
	/// implementations using the fluent <see cref="IIdentityBuilder.AddProvisioner{TProvisioner}"/>
	/// method. The same callback applies to instance keys across both providers —
	/// just pair each app-provided provisioner with its configured instance name.
	/// </param>
	/// <returns>The host application builder for chaining.</returns>
	/// <example>
	/// <code>
	/// builder.AddIdentity(p => p
	///     .AddProvisioner&lt;ClientABorrowerProvisioner&gt;("clientA_descope")    // Oidc instance
	///     .AddProvisioner&lt;EmployeeProvisioner&gt;("primary"));                  // EntraExternalId instance
	/// </code>
	/// </example>
	public static IHostApplicationBuilder AddIdentity(
		this IHostApplicationBuilder builder,
		Action<IIdentityBuilder>? configure = null) {

		// Marker dedup — provider registration runs once regardless of repeat calls.
		// The configure callback always runs so repeated calls can still add provisioners.
		if (!builder.Services.IsMarkerTypeRegistered<AddIdentityMarker>()) {
			builder.Services.MarkTypeAsRegistered<AddIdentityMarker>();

			builder.RegisterIdentityProvider<
				OidcIdentityProviderRegistrar,
				OidcIdentityProviderSettings,
				OidcIdentityProviderInstanceSettings>();

			builder.RegisterIdentityProvider<
				EntraExternalIdIdentityProviderRegistrar,
				EntraExternalIdIdentityProviderSettings,
				EntraExternalIdIdentityProviderInstanceSettings>();
		}

		configure?.Invoke(new IdentityBuilder(builder));
		return builder;
	}
}
