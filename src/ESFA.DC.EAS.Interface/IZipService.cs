using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.EAS.Interface
{
    public interface IZipService
    {
        Task CreateZipAsync(string zipName, IEnumerable<string> fileNames, string container, CancellationToken cancellationToken);
    }
}
