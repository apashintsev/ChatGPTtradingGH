using ChatGPTtrading.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ChatGPTtrading.Infrastructure.ModelConfigurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.Property(rp => rp.Id).IsRequired().ValueGeneratedOnAdd();

            builder
                .HasOne(r => r.User)
                .WithMany(u => u.Documents)
                .HasForeignKey(r => r.UserId);
        }
    }
}
