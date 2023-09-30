using ChatGPTtrading.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ChatGPTtrading.Infrastructure.ModelConfigurations
{
    public class ReferralProfitConfiguration : IEntityTypeConfiguration<ReferralProfit>
    {
        public void Configure(EntityTypeBuilder<ReferralProfit> builder)
        {
            builder
                .HasOne(rp => rp.User)
                .WithMany(u => u.ReferralProfits)
                .HasForeignKey(rp => rp.UserId);

            builder
                .HasOne(rp => rp.Referral)
                .WithMany()
                .HasForeignKey(rp => rp.ReferralId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(rp => rp.Id).IsRequired().ValueGeneratedOnAdd();
        }
    }
}
