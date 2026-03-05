using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasConversion(
                firstName => firstName.Value,
                value => new FirstName(value))
            .HasMaxLength(200);

        builder.Property(x => x.LastName)
            .HasConversion(
                lastName => lastName.Value,
                value => new LastName(value))
            .HasMaxLength(200);

        builder.Property(x => x.Email)
            .HasConversion(
                email => email.Value,
                value => new Domain.Users.Email(value))
            .HasMaxLength(400);

        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasIndex(user => user.IdentityId).IsUnique();
    }
}