using ChatGPTtrading.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ChatGPTtrading.Infrastructure.ModelConfigurations;

public class FakeActivityConfiguration : IEntityTypeConfiguration<FakeActivity>
{
    public void Configure(EntityTypeBuilder<FakeActivity> builder)
    {
        builder.Property(rp => rp.Id).IsRequired().ValueGeneratedOnAdd();
    }
}
