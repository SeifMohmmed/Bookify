using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bookify.Api.OpenApi;
/// <summary>
/// This class configures Swagger documentation for each API version.
/// It integrates Swagger with ASP.NET API Versioning.
/// </summary>
public sealed class ConfigureSwaggerOptions(
    IApiVersionDescriptionProvider provider) : IConfigureNamedOptions<SwaggerGenOptions>
{
    /// <summary>
    /// Configure Swagger options for a named configuration.
    /// This simply calls the main Configure method.
    /// </summary>
    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    /// <summary>
    /// Configure Swagger documents for each API version discovered by ApiExplorer.
    /// For example: v1, v2, etc.
    /// </summary>
    public void Configure(SwaggerGenOptions options)
    {
        // Loop through all API version descriptions
        foreach (var description in provider.ApiVersionDescriptions)
        {
            // Create a separate Swagger document for each version
            options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
        }
    }

    /// <summary>
    /// Creates metadata information for each API version.
    /// This information appears in the Swagger UI.
    /// </summary>
    private static OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
    {
        var openApiInfo = new OpenApiInfo
        {
            // Title shown in Swagger UI
            Title = $"Bookify.Api v{description.ApiVersion}",

            // Version number of the API
            Version = description.ApiVersion.ToString()
        };

        // If the API version is deprecated, show a warning in Swagger
        if (description.IsDeprecated)
            openApiInfo.Description += "This API version has been deprecated.";

        return openApiInfo;
    }
}