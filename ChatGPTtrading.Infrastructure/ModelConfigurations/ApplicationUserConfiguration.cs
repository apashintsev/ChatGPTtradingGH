using ChatGPTtrading.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatGPTtrading.Infrastructure.ModelConfigurations
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x=>x.Id).IsRequired().ValueGeneratedNever();
            builder.HasOne<User>()
                    .WithOne()
                    .HasForeignKey<ApplicationUser>(u => u.UserId)
                    .IsRequired();

        }
    }
}
