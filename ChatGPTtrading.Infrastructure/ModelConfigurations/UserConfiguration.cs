using ChatGPTtrading.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ChatGPTtrading.Infrastructure.ModelConfigurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Id).IsRequired(true).ValueGeneratedOnAdd();
            builder.Property(x => x.Email).IsRequired(true).HasColumnType("text");
            builder.Property(x => x.Phone).IsRequired(true).HasColumnType("text");
            builder.Property(x => x.FirstName).IsRequired(false).HasColumnType("text");
            builder.Property(x => x.LastName).IsRequired(false).HasColumnType("text");
            builder.Property(x => x.Residency).IsRequired(false).HasColumnType("text");
            builder.Property(x => x.Citizenship).IsRequired(false).HasColumnType("text");
            builder.Property(x => x.Occupation).IsRequired(false).HasColumnType("text");
            builder.Property(x => x.TemporaryResidencePermit).IsRequired(false).HasColumnType("text");
            builder.Property(x => x.KycStatus).IsRequired(true).HasColumnType("boolean");
            builder.Property(x => x.ReferralLink).IsRequired(false).HasColumnType("text");
            builder.Property(x=>x.AvatarUrl).IsRequired(false).HasColumnType("text");

            builder
                .HasOne(u => u.UserAccount)
                .WithOne(ua => ua.User)
                .HasForeignKey<UserAccount>(ua => ua.UserId);

        }
    }
}
