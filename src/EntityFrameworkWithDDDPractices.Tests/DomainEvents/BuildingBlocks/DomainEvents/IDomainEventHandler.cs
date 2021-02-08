using System.Threading.Tasks;

namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.DomainEvents
{

    public interface IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        Task HandleAsync(TDomainEvent domainEvent);
    }
}