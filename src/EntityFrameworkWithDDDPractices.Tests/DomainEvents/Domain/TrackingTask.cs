using EntityFrameworkWithDDDPractices.Tests.DomainEvents.Domain.Events;
using System;

namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.Domain
{
    public class TrackingTask : EntityBase
    {
        public TrackingTaskStatus Status { get; private set; } = TrackingTaskStatus.Ready;

        public void Start()
        {
            if (Status != TrackingTaskStatus.Ready)
            {
                throw new InvalidOperationException("...");
            }
            Status = TrackingTaskStatus.InProgress;
            AddDomainEvent(new TrackingTaskStartedEvent(Id));
        }

        public void Complete()
        {
            if (Status != TrackingTaskStatus.InProgress)
            {
                throw new InvalidOperationException("...");
            }
            Status = TrackingTaskStatus.Completed;
            AddDomainEvent(new TrackingTaskCompletedEvent(Id));
        }

        public void Cancel()
        {
            if (Status != TrackingTaskStatus.InProgress)
            {
                throw new InvalidOperationException("...");
            }
            Status = TrackingTaskStatus.Canceled;
            AddDomainEvent(new TrackingTaskCanceledEvent(Id));
        }
    }


    public enum TrackingTaskStatus
    {
        //Pending,
        Ready,
        InProgress,
        Completed,
        Canceled
    }
}
