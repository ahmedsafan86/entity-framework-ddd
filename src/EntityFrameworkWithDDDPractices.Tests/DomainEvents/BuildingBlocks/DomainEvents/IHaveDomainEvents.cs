namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.DomainEvents
{
    using System.Collections.Generic;

    public interface IHaveDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    }
}
