using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.CustomerName).IsRequired().HasMaxLength(100);
        builder.Property(o => o.ProductName).IsRequired().HasMaxLength(100);
        builder.Property(o => o.Amount).HasColumnType("numeric(18,2)");
        builder.Property(o => o.Status).IsRequired().HasMaxLength(20);
        builder.Property(o => o.CreatedAt);
        builder.Property(o => o.ProcessedAt);
    }
}
