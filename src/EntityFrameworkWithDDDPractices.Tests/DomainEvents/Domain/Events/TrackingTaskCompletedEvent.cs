using EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.DomainEvents;

namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.Domain.Events
{
    public class TrackingTaskCompletedEvent : IDomainEvent
    {
        public int TrackingTaskId { get; }

        public TrackingTaskCompletedEvent(int trackingTaskId)
        {
            TrackingTaskId = trackingTaskId;
        }
    }
}
