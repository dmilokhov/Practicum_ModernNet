using BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Infrastructure.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(b => b.Topic)
            .HasColumnName("topic")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Key)
            .HasColumnName("key")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Payload)
            .HasColumnName("payload")
            .IsRequired();

        builder.Property(b => b.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(b => b.IsProcessed)
            .HasColumnName("is_processed")
            .IsRequired();
    }
}
