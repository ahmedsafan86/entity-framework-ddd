using EntityFrameworkWithDDDPractices.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.DomainEvents
{
    internal interface IDomainEventDispatcher
    {
        Task DispatchEventAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
    }

    internal class DomainEventDispatcher : IDomainEventDispatcher
    {
        private static readonly MethodInfo _internalDispatchEventAsyncMethodInfo =
            ReflectionHelper.GetPrivateStaticMethod<DomainEventDispatcher>(nameof(InternalDispatchEventAsync));
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Task DispatchEventAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            return _internalDispatchEventAsyncMethodInfo.MakeGenericMethod(domainEvent.GetType())
                .Invoke(this, new object[] { domainEvent }) as Task;
        }

        private async Task InternalDispatchEventAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            using var scope = _serviceProvider.CreateScope();
            var handlers = scope.ServiceProvider.GetServices<IDomainEventHandler<TDomainEvent>>();
            foreach (var handler in handlers)
            {
                try
                {
                    await handler.HandleAsync(domainEvent);
                }
                catch (Exception excption)
                {
                    // TODO: log exception
                }
            }
        }
    }
}