namespace Microsoft.Extensions.Hosting;

using Cirreum.Identity;
using Cirreum.Identity.Provisioning;

/// <summary>
/// App-facing umbrella extensions for the Cirreum Identity provider family — composes
/// <see cref="Cirreum.Identity.Oidc"/> and <see cref="Cirreum.Identity.EntraExternalId"/>
/// Runtime Extensions packages behind a single <c>AddIdentity()</c> entry point.
/// </summary>
public static class HostApplicationBuilderExtensions {

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

		// Register both providers without passing the configure callback — we invoke it
		// exactly once below against a single IdentityBuilder so AddProvisioner calls run
		// once per key (not per provider × key).
		builder.AddOidcIdentity();
		builder.AddEntraExternalIdIdentity();

		configure?.Invoke(new IdentityBuilder(builder));
		return builder;
	}
}
