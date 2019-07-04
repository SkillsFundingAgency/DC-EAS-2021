using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.EAS1920.EF.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.EAS.DataService
{
    public class EasPaymentService : IEasPaymentService
    {
        private readonly IEasdbContext _repository;

        public EasPaymentService(IEasdbContext repository)
        {
            _repository = repository;
        }

        public async Task<List<PaymentType>> GetAllPaymentTypes(CancellationToken cancellationToken)
        {
            var paymentTypes = await _repository.PaymentTypes.Include(x => x.FundingLine).Include(x => x.AdjustmentType).OrderBy(s => s.PaymentId).ThenBy(s => s.PaymentName).ToListAsync(cancellationToken);
            return paymentTypes;
        }
    }
}