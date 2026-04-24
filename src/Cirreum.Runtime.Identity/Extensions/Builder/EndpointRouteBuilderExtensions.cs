namespace Microsoft.AspNetCore.Builder;

using Cirreum.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// App-facing umbrella extensions for mapping every Cirreum Identity provisioning
/// endpoint across all registered providers.
/// </summary>
public static class EndpointRouteBuilderExtensions {

	/// <summary>
	/// Maps every enabled provisioning route across every registered Cirreum Identity
	/// provider (Oidc and EntraExternalId). Invokes every
	/// <see cref="IdentityProviderMapping"/> stashed in DI — unfiltered.
	/// </summary>
	/// <param name="endpoints">The endpoint route builder.</param>
	/// <returns>The endpoint route builder for chaining.</returns>
	/// <remarks>
	/// Call after <c>app.UseAuthentication()</c> / <c>app.UseAuthorization()</c>. Each
	/// instance's route is mapped as anonymous (<c>AllowAnonymous</c>) and excluded from
	/// OpenAPI — inbound authentication (shared-secret for Oidc, JWT for EntraExternalId)
	/// is performed internally by each provider's handler.
	/// </remarks>
	public static IEndpointRouteBuilder MapIdentity(this IEndpointRouteBuilder endpoints) {

		var mappings = endpoints.ServiceProvider.GetServices<IdentityProviderMapping>();
		foreach (var mapping in mappings) {
			mapping.Map(endpoints);
		}

		return endpoints;
	}
}
