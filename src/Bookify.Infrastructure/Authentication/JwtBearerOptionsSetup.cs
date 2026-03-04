using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authentication;
/// <summary>
/// Configures JwtBearerOptions using values from AuthenticationOptions.
/// This allows JWT settings to be loaded from configuration (appsettings.json).
/// </summary>
public class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    // Holds authentication configuration loaded from appsettings
    private readonly AuthenticationOptions _options;

    /// <summary>
    /// Constructor receives AuthenticationOptions through IOptions pattern
    /// </summary>
    public JwtBearerOptionsSetup(IOptions<AuthenticationOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Configures JWT bearer authentication options
    /// </summary>
    public void Configure(JwtBearerOptions options)
    {
        // Expected audience (API identifier)
        options.Audience = _options.Audience;

        // Keycloak metadata endpoint for discovering auth configuration
        options.MetadataAddress = _options.MetadataUrl;

        // Determines if HTTPS is required for metadata endpoint
        options.RequireHttpsMetadata = _options.RequireHttpsMetadata;

        // Valid issuer (usually Keycloak realm URL)
        options.TokenValidationParameters.ValidIssuer = _options.ValidIssuer;
    }

    /// <summary>
    /// Named options configuration (required by IConfigureNamedOptions)
    /// </summary>
    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}