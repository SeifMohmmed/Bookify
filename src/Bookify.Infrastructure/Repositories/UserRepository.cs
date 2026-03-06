using Bookify.Domain.Users;

namespace Bookify.Infrastructure.Repositories;
internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public override void Add(User user)
    {
        // Tell EF Core that these role entities already exist in the database
        // so it doesn't try to insert them again.
        foreach (var role in user.Roles)
        {
            context.Attach(role);
        }

        context.Add(user);
    }
}
