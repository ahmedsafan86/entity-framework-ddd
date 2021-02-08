using EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.DomainEvents;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.Infrastructure
{

    internal class DbContextSupportingDomainEvents : DbContext
    {
        private readonly IDomainEventStore _domainEventStore;

        public DbContextSupportingDomainEvents(DbContextOptions options, IDomainEventStore domainEventStore)
            : base(options)
        {
            _domainEventStore = domainEventStore;
        }


        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            // Neglecting value of async operations, to take value use SaveChangesAsync
            OnAfterSaveChanges().GetAwaiter().GetResult();
            return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            await OnAfterSaveChanges();
            return result;
        }

        protected virtual Task OnAfterSaveChanges()
        {
            var domainEvntsTasks = ChangeTracker.Entries<IHaveDomainEvents>()
                .SelectMany(entry => entry.Entity.DomainEvents)
                .Select(domainEvent => _domainEventStore.RaiseAsync(domainEvent))
                .ToArray();
            return Task.WhenAll(domainEvntsTasks);
        }
    }
}
