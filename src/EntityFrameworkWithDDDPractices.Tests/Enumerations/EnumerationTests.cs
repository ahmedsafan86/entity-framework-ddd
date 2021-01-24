using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using Xunit;

namespace EntityFrameworkWithDDDPractices.Tests.Enumerations
{
    public class EnumerationTests
    {
        private readonly ServiceProvider _serviceProvider;

        public EnumerationTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void UseEnumerationTypes()
        {
            using (var scope1 = _serviceProvider.CreateScope())
            {
                var dbContext1 = scope1.ServiceProvider.GetRequiredService<EnumerationDbContext>();
                dbContext1.Database.EnsureCreated();
                dbContext1.DriverStatuses.Any().ShouldBeTrue();
                var originalStatusCount = dbContext1.DriverStatuses.Count();
                var busyDriver = new Driver
                {
                  Name = "The Busy one",
                  Status = DriverStatus.Busy
                };
                var availableDriver = new Driver
                {
                  Name = "The Available one",
                  Status = DriverStatus.Available
                };
                dbContext1.AddRange(busyDriver, availableDriver);
                dbContext1.SaveChanges();
                using (var scope2 = _serviceProvider.CreateScope())
                {
                    var dbContext2 = scope2.ServiceProvider.GetRequiredService<EnumerationDbContext>();
                    dbContext2.DriverStatuses.Count().ShouldBe(originalStatusCount);
                    dbContext2.Drivers.Count().ShouldBe(2);
                    dbContext2.Drivers.Count(driver=>driver.Status == DriverStatus.Available).ShouldBe(1);
                    dbContext2.Drivers.Count(driver=>driver.Status == DriverStatus.Busy).ShouldBe(1);
                    dbContext2.Drivers.Count(driver=>driver.Status == DriverStatus.Offline).ShouldBe(0);
                }
            }
        }
    }
}