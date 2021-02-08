namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.Domain
{
    using EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.DomainEvents;
    using System.Collections.Generic;

    public class EntityBase : IHaveDomainEvents
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public int Id { get; private set; }

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}
