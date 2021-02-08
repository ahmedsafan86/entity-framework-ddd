using EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.BackgroundJobs;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.DomainEvents
{
    public interface IDomainEventStore
    {
        Task RaiseAsync<TDomainEvent>([NotNull] TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
    }

    public class DomainEventStoreOptions
    {
        public bool DispatchInBackground { get; set; }
    }

    internal class DomainEventStore : IDomainEventStore
    {
        private readonly DomainEventStoreOptions _options;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly IBackgroundJobManager _jobManager;

        public DomainEventStore(DomainEventStoreOptions options, IDomainEventDispatcher domainEventDispatcher, IBackgroundJobManager jobManager)
        {
            _options = options;
            _domainEventDispatcher = domainEventDispatcher;
            _jobManager = jobManager;
        }

        public Task RaiseAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            return InternalRaise(domainEvent);
        }

        private Task InternalRaise<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            var result = Task.CompletedTask;
            if (_options.DispatchInBackground)
            {
                _jobManager.Enqueue(new DomainEventDispatcherJobModel { DomainEvent = domainEvent });
            }
            else
            {
                result = _domainEventDispatcher.DispatchEventAsync(domainEvent);
            }
            return result;
        }
    }
}