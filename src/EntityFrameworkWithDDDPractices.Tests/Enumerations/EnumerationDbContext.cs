using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkWithDDDPractices.Tests.Enumerations
{
    public class EnumerationDbContext : DbContext
    {
        public DbSet<Driver> Drivers { get; set; }

        public DbSet<DriverStatus> DriverStatuses { get; set; }

        public EnumerationDbContext([NotNullAttribute] DbContextOptions options)
          : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DriverStatus>().HasData(Enumeration.GetAll<DriverStatus>());
            modelBuilder.Entity<DriverStatus>()
                .Property(ct => ct.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            BeforeSaveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            BeforeSaveChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void BeforeSaveChanges()
        {
            ChangeTracker.Entries()
                .Where(entry => entry.Metadata.ClrType.IsAssignableTo(typeof(Enumeration)))
                .ToList()
                .ForEach(Entry => Entry.State = EntityState.Unchanged);
        }
    }

    public class Driver
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DriverStatus Status { get; set; }
    }

    public class DriverStatus : Enumeration
    {
        public static DriverStatus Busy = new DriverStatus(1, "Busy");
        public static DriverStatus Available = new DriverStatus(2, "Available");

        public static DriverStatus Offline = new DriverStatus(3, "Offline");

        public DriverStatus(int id, string name)
          : base(id, name)
        {

        }
    }
    public static class Extensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services)
        {
            services.AddDbContext<EnumerationDbContext>(options =>
            {
                const string connectionString = "Data Source=InMemorySample;Mode=Memory;Cache=Shared";
                var masterConnection = new SqliteConnection(connectionString);
                masterConnection.Open();
                options.UseSqlite(masterConnection);
            });
            return services;
        }
    }
}