using ChatGPTtrading.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ChatGPTtrading.Infrastructure.ModelConfigurations
{
    public class ReferralConfiguration : IEntityTypeConfiguration<Referral>
    {
        public void Configure(EntityTypeBuilder<Referral> builder)
        {
            builder.Property(rp => rp.Id).IsRequired().ValueGeneratedOnAdd();

            builder
                .HasOne(r => r.User)
                .WithMany(u => u.Referrals)
                .HasForeignKey(r => r.UserId);

            builder
                .HasOne(r => r.ReferralUser)
                .WithMany()
                .HasForeignKey(r => r.ReferralUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
