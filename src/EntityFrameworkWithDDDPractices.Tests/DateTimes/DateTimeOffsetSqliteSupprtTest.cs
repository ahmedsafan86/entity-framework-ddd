using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace EntityFrameworkWithDDDPractices.Tests.DateTimes
{
    public class DateTimeOffsetSqliteSupprtTest
    {
        [Fact]
        public async Task DateTimeOffsetShouldBestoredInSqliteAsync()
        {
            var services = new ServiceCollection();
            const string connectionString = "Data Source=InMemoryDomainEventsSample;Mode=Memory;Cache=Shared";
            var dbConnection = new SqliteConnection(connectionString);
            dbConnection.Open();
            services.AddDbContext<DateTimeDbContext>(options => options.UseSqlite(dbConnection));
            var serviceProvider = services.BuildServiceProvider();
            using (var scope1 = serviceProvider.CreateScope())
            {
                var dbContext = scope1.ServiceProvider.GetRequiredService<DateTimeDbContext>();
                await dbContext.Database.EnsureCreatedAsync();
                //create DateTimeOffset with different timezones and store
                var dates = new[]
                {
                    DateTimeOffset.MaxValue,
                    DateTimeOffset.MinValue,
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.Now.AddMinutes(1),
                    new DateTimeOffset(DateTime.UtcNow.Ticks, TimeSpan.FromHours(3)),
                };

                dbContext.AddRange(dates.Select(date => new TestEntity { DateTimeOffset = date }));
                await dbContext.SaveChangesAsync();
                dbContext.ChangeTracker.Clear();
                var dbDates = await dbContext.TestEntities.Select(entity => entity.DateTimeOffset).ToArrayAsync();
                dbDates.ShouldBe(dates);

                foreach (var date in dates)
                {
                    dbContext.TestEntities.Count(entity => entity.DateTimeOffset == date).ShouldBe(1);
                }

                var sortedDbDates = await dbContext.TestEntities
                    .OrderBy(entity => entity.DateTimeOffset)
                    .Select(entity => entity.DateTimeOffset)
                    .ToArrayAsync();
                var orderQuery = dbContext.TestEntities
                    .OrderBy(entity => entity.DateTimeOffset)
                    .Select(entity => entity.DateTimeOffset)
                    .ToQueryString();
                var sortedDates = dates.OrderBy(date => date);
                sortedDbDates.ShouldBe(sortedDates);
                dbContext.TestEntities.Count().ShouldBe(dbContext.TestEntities.Count(entity => entity.OptionalDateTimeOffset == null));
            }
        }


        internal class TestEntity
        {
            public int Id { get; set; }

            public DateTimeOffset DateTimeOffset { get; set; }

            public DateTimeOffset? OptionalDateTimeOffset { get; set; }

            public DateTime DateTime { get; set; }

            public DateTime? OptionalDateTime { get; set; }
        }

        internal class DateTimeDbContext : DbContext
        {
            public DbSet<TestEntity> TestEntities { get; set; }

            public DateTimeDbContext([NotNull] DbContextOptions options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<TestEntity>()
                    .Property(entity => entity.DateTimeOffset)
                    .HasConversion(new DateTimeOffsetToUtcDateTimeConverter());
                modelBuilder.Entity<TestEntity>()
                    .Property(entity => entity.OptionalDateTimeOffset)
                    .HasConversion(new DateTimeOffsetToUtcDateTimeConverter());
                modelBuilder.Entity<TestEntity>()
                    .Property(entity => entity.DateTime)
                    .HasConversion(new DateTimeToUtcConverter());
                modelBuilder.Entity<TestEntity>()
                    .Property(entity => entity.OptionalDateTime)
                    .HasConversion(new DateTimeToUtcConverter());
            }

            private class DateTimeOffsetToUtcDateTimeConverter : ValueConverter<DateTimeOffset, DateTime>
            {
                private static readonly Expression<Func<DateTimeOffset, DateTime>> _convertToExpression =
                    value => value.UtcDateTime;
                private static readonly Expression<Func<DateTime, DateTimeOffset>> _convertFromExpression =
                    value => new DateTimeOffset(value, TimeSpan.Zero);

                public DateTimeOffsetToUtcDateTimeConverter()
                    : base(_convertToExpression, _convertFromExpression)
                {
                }
            }

            private class DateTimeToUtcConverter : ValueConverter<DateTime, DateTime>
            {
                private static readonly Expression<Func<DateTime, DateTime>> _convertToExpression =
                    value => value.ToUniversalTime();
                private static readonly Expression<Func<DateTime, DateTime>> _convertFromExpression =
                    value => DateTime.SpecifyKind(value, DateTimeKind.Utc);

                public DateTimeToUtcConverter()
                    : base(_convertToExpression, _convertFromExpression)
                {
                }
            }
        }
    }

#if false
    internal class MilliSecondAccuracyDateTimeComparer : IEqualityComparer<DateTimeOffset>
    {

        bool IEqualityComparer<DateTimeOffset>.Equals(DateTimeOffset x, DateTimeOffset y)
        {
            return RemoveResolution(x.Ticks) == RemoveResolution(y.Ticks);
        }

        int IEqualityComparer<DateTimeOffset>.GetHashCode(DateTimeOffset obj)
        {
            return RemoveResolution(obj.Ticks / 1000).GetHashCode();
        }
        /// <summary>
        /// set first 4 digits to zero
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        private long RemoveResolution(long ticks)
        {
            return ticks / 1000 * 1000;
        }
    } 
#endif
}
