using EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.BackgroundJobs;
using EntityFrameworkWithDDDPractices.Tests.Helpers;
using System.Reflection;
using System.Threading.Tasks;

namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.DomainEvents
{
    internal class DomainEventDispatcherJobModel
    {
        public IDomainEvent DomainEvent { get; set; }
    }

    internal class DomainEventDispatcherJob : IBackgroundJob<DomainEventDispatcherJobModel>
    {
        private static readonly MethodInfo _dispatchMethodInfo =
            ReflectionHelper.GetPrivateStaticMethod<DomainEventDispatcher>(nameof(DispatchAsync));
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public DomainEventDispatcherJob(IDomainEventDispatcher domainEventDispatcher)
        {
            _domainEventDispatcher = domainEventDispatcher;
        }

        public Task ExecuteAsync(DomainEventDispatcherJobModel jobModel)
        {
            return _dispatchMethodInfo.MakeGenericMethod(jobModel.DomainEvent.GetType())
                .Invoke(null, new object[] { jobModel.DomainEvent }) as Task;
        }

        private static Task DispatchAsync<TDomainEvent>(
            IDomainEventDispatcher dispatcher,
            TDomainEvent domainEvent)
            where TDomainEvent : IDomainEvent
        {
            return dispatcher.DispatchEventAsync(domainEvent);
        }
    }
}