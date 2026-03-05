using Bookify.Domain.Users;

namespace Bookify.Infrastructure.Authentication.Models;
/// <summary>
/// Represents a Keycloak user representation.
/// This model matches the structure used by the Keycloak Admin REST API
/// when creating or retrieving users.
/// </summary>
public sealed class UserRepresentationModel
{
    public Dictionary<string, string> Access { get; set; }

    public Dictionary<string, List<string>> Attributes { get; set; }

    public Dictionary<string, string> ClientRoles { get; set; }

    public long? CreatedTimestamp { get; set; }

    public CredentialRepresentationModel[] Credentials { get; set; }

    public string[] DisableableCredentialTypes { get; set; }

    public string Email { get; set; }

    public bool? EmailVerified { get; set; }

    public bool? Enabled { get; set; }    // Indicates whether the user account is enabled.

    public string FederationLink { get; set; }    // Federation link if the user comes from an external identity provider.

    public string Id { get; set; }

    public string[] Groups { get; set; }  // Groups that the user belongs to.

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int? NotBefore { get; set; }   // Defines a time before which tokens are not valid.

    public string Origin { get; set; }   // Source of the user (e.g., external provider).

    public string[] RealmRoles { get; set; }  // Roles assigned at the realm level.

    public string[] RequiredActions { get; set; }  // Actions required from the user (e.g., update password).

    public string Self { get; set; }  // Self reference URL in Keycloak.

    public string ServiceAccountClientId { get; set; } // Client ID if this user is a service account.

    public string Username { get; set; }

    /// <summary>
    /// Maps a domain User entity to a Keycloak user representation model.
    /// Used when creating a user in Keycloak.
    /// </summary>
    internal static UserRepresentationModel FromUser(User user) =>
        new()
        {
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value,
            Email = user.Email.Value,
            Username = user.Email.Value,
            Enabled = true,
            EmailVerified = true,
            CreatedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Attributes = new Dictionary<string, List<string>>(),
            RequiredActions = Array.Empty<string>()
        };
}