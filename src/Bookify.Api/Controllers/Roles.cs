namespace Bookify.Api.Controllers;
/// <summary>
/// Contains the role names used for authorization in the API layer.
/// These constants are used in attributes like [Authorize(Roles = ...)].
/// </summary>
public static class Roles
{
    public const string Registered = "Registered";
}
