using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.EAS.Interface
{
    public interface IEasServiceTask
    {
        string TaskName { get; }

        Task ExecuteAsync(IEasJobContext easContext, CancellationToken cancellationToken);
    }
}
