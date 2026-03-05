using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Dapper;

namespace Bookify.Application.Users.GetLoggedInUser;
/// <summary>
/// Handles the query for retrieving the currently authenticated user.
/// </summary>
internal sealed class GetLoggedInUserQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetLoggedInUserQuery, UserResponse>
{
    /// <summary>
    /// Handles the query request and returns the logged-in user's data.
    /// </summary>
    public async Task<Result<UserResponse>> Handle(
        GetLoggedInUserQuery request,
        CancellationToken cancellationToken)
    {
        // Create a database connection using the SQL connection factory
        using var connection = sqlConnectionFactory.CreateConnection();

        // SQL query to retrieve the user by IdentityId
        const string sql = """
            SELECT
                id AS Id,
                first_name AS FirstName,
                last_name AS LastName,
                email AS Email
            FROM users
            WHERE identity_id = @IdentityId
            """;

        // Execute the query using Dapper and map the result to UserResponse
        var user = await connection.QuerySingleAsync<UserResponse>(
            sql,
            new
            {
                // IdentityId retrieved from the authenticated user context
                userContext.IdentityId
            });

        // Return the result wrapped inside the Result object
        return user;
    }
}