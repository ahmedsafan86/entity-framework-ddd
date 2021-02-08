using EntityFrameworkWithDDDPractices.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.DomainEvents
{
    public static class Extensions
    {
        public static IServiceCollection AddDomainEvents(this IServiceCollection services, IConfiguration configuration, DomainEventStoreOptions options = default)
        {
            // register our Implementation of the domainEventStore
            services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddSingleton<IDomainEventStore, DomainEventStore>();
            services.AddOptions();
            services.Configure<DomainEventStoreOptions>(configuration.GetSection("qq"));
            //services.ConfigureOptions(options ?? new DomainEventStoreOptions());
            // register all types of IDomainEventHandler
            var domainEventHandlerTypes = ReflectionHelper.GetAllConcreteTypesImplementingGenericInterface(typeof(IDomainEventHandler<>));
            foreach (var (handlerType, domainEventType) in domainEventHandlerTypes)
            {
                services.AddScoped(typeof(IDomainEventHandler<>).MakeGenericType(domainEventType), handlerType);
            }
            return services;
        }
    }
}