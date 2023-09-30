using ChatGPTtrading.Domain.Entities;
using ChatGPTtrading.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace ChatGPTtrading.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserAccount> UsersAccount { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Activity> Activitys { get; set; }
    public DbSet<Referral> Referrals { get; set; }
    public DbSet<ReferralProfit> Profits { get; set; }
    public DbSet<Withdrawal> Withdrawals { get; set; }
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<PlatformSettings> PlatformSettings { get; set; }
    public DbSet<FileItem> Files { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<FakeStats> FakeStats { get; set; }
    public DbSet<FakeActivity> FakeActivities{ get;set;}
    public DbSet<Invoice> Invoices { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opt) : base(opt)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Configure primary key as Guid for all entities
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (entityType.BaseType == null && entityType.FindPrimaryKey() != null)
            {
                entityType.FindPrimaryKey().Properties
                    .Where(p => p.ClrType == typeof(Guid))
                    .ToList()
                    .ForEach(p => p.ValueGenerated = ValueGenerated.OnAdd);
            }
        }

        builder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly(),
            t => t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
        );


        builder.HasPostgresEnum<TransactionStatus>();
        //TODO: Currency to PostgresEnum

        base.OnModelCreating(builder);
    }
}
