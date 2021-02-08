using EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.DomainEvents;

namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.Domain.Events
{
    public class TrackingTaskStartedEvent : IDomainEvent
    {
        public int TrackingTaskId { get; }

        public TrackingTaskStartedEvent(int trackingTaskId)
        {
            TrackingTaskId = trackingTaskId;
        }
    }
}
