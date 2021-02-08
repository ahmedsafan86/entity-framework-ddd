using EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.DomainEvents;
using EntityFrameworkWithDDDPractices.Tests.DomainEvents.Domain.Events;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents
{
    public class DomainEventsTest
    {
        // create mocks for IJobManager
        // create serviceProvider, adddomainEvents, adddbcontext
        // create multiple tasks, change their status, make sure domain event handlers are invoked, also in case of run in background IJobManager.Enque should be called with appropriate model

        [Fact]
        public async Task SaveChangesAsync_ShouldAddEventsToStore()
        {
            // Arrange
            var services = new ServiceCollection();
            const string connectionString = "Data Source=InMemoryDomainEventsSample;Mode=Memory;Cache=Shared";
            var dbConnection = new SqliteConnection(connectionString);
            dbConnection.Open();
            services.AddDbContext<DomainEventsDbContext>(options => options.UseSqlite(dbConnection));
            var configuration = new ConfigurationBuilder().Build();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddDomainEvents(configuration);
            var domainEventStoreMock = new Mock<IDomainEventStore>();
            services.Replace(ServiceDescriptor.Scoped<IDomainEventStore>((_) => domainEventStoreMock.Object));
            using var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DomainEventsDbContext>();
                await dbContext.Database.EnsureCreatedAsync();
                dbContext.TrackingTasks.AddRange(
                    new[]
                    {
                        new Domain.TrackingTask(),
                        new Domain.TrackingTask()
                    });
                await dbContext.SaveChangesAsync();
            }
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DomainEventsDbContext>();
                var tasks = await dbContext.TrackingTasks.Where(task => task.Status == Domain.TrackingTaskStatus.Ready).ToArrayAsync();
                foreach (var task in tasks)
                {
                    task.Start();
                }
                await dbContext.SaveChangesAsync();
                foreach (var task in tasks)
                {
                    domainEventStoreMock.Verify(
                        mock => mock.RaiseAsync(
                            It.Is<IDomainEvent>(@event =>
                                @event is TrackingTaskStartedEvent &&
                                (@event as TrackingTaskStartedEvent).TrackingTaskId == task.Id)),
                        Times.Once);
                }
            }
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DomainEventsDbContext>();
                var task = await dbContext.TrackingTasks.FirstAsync(task => task.Status == Domain.TrackingTaskStatus.InProgress);
                task.Complete();
                await dbContext.SaveChangesAsync();
                domainEventStoreMock.Verify(mock => mock.RaiseAsync(
                    It.Is<IDomainEvent>(@event =>
                        @event is TrackingTaskCompletedEvent &&
                        (@event as TrackingTaskCompletedEvent).TrackingTaskId == task.Id)),
                    Times.Once);
            }
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DomainEventsDbContext>();
                var task = await dbContext.TrackingTasks.FirstAsync(task => task.Status == Domain.TrackingTaskStatus.InProgress);
                task.Cancel();
                await dbContext.SaveChangesAsync();
                domainEventStoreMock.Verify(mock =>
                    mock.RaiseAsync(
                        It.Is<IDomainEvent>(@event =>
                            @event is TrackingTaskCanceledEvent &&
                            (@event as TrackingTaskCanceledEvent).TrackingTaskId == task.Id)),
                        Times.Once);
            }
        }

        //public async Task SaveChangesAsync_ConfiguredForBackground_ShouldFireEventsAsEnqueuedJobs()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
