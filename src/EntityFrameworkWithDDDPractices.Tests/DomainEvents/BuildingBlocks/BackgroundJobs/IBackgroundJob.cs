using System.Threading.Tasks;

namespace EntityFrameworkWithDDDPractices.Tests.DomainEvents.BuildingBlocks.BackgroundJobs
{
    public interface IBackgroundJob<TJobModel> where TJobModel : class
    {
        Task ExecuteAsync(TJobModel jobModel);
    }
}