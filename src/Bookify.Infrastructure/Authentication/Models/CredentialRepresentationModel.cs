namespace Bookify.Infrastructure.Authentication.Models;
/// <summary>
/// Represents a credential object in Keycloak.
/// Typically used for setting user passwords.
/// </summary>
public sealed class CredentialRepresentationModel
{
    public string Value { get; set; }   // The actual credential value (e.g., the password)

    /// <summary>
    /// Indicates whether the credential is temporary.
    /// If true, the user will be required to change it on first login.
    /// </summary>
    public bool Temporary { get; set; }

    public string Type { get; set; }  // Type of the credential (usually "password")
}