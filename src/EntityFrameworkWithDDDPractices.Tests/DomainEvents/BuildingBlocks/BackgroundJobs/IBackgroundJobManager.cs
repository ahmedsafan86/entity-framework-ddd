namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.BackgroundJobs
{
    public interface IBackgroundJobManager
    {
        void Enqueue<TJobModel>(TJobModel jobModel);
    }
}