using EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.DomainEvents;
using EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.Infrastructure;
using EntityFrameworkWithDDDPractices.Tests.DomainEvents.Domain;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents
{
    internal class DomainEventsDbContext : DbContextSupportingDomainEvents
    {
        public DbSet<TrackingTask> TrackingTasks { get; set; }

        public DomainEventsDbContext(DbContextOptions options, IDomainEventStore domainEventStore)
            : base(options, domainEventStore)
        {
        }
    }
}
