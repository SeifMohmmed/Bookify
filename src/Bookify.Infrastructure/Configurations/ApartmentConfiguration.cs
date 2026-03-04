using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;
internal sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.ToTable("apartments");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Address);

        builder.Property(x => x.Name)
            .HasConversion(
                name => name.Value,
                value => new Name(value))
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasConversion(
                description => description.Value,
                value => new Description(value))
            .HasMaxLength(2000);

        builder.OwnsOne(x => x.Price, priceBuilder =>
        {
            priceBuilder.Property(money => money.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });

        builder.OwnsOne(x => x.CleaningFee, priceBuilder =>
        {
            priceBuilder.Property(money => money.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });

        //Optimistic Concurrency Support
        builder.Property<uint>("Version").IsRowVersion();
    }
}