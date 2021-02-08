using EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.DomainEvents;

namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.Domain.Events
{
    public class TrackingTaskCanceledEvent : IDomainEvent
    {
        public int TrackingTaskId { get; }

        public TrackingTaskCanceledEvent(int trackingTaskId)
        {
            TrackingTaskId = trackingTaskId;
        }
    }
}
