using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.EAS1819.Data;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Services.Interface.Data;

namespace ESFA.DC.EAS1819.Services.Data
{
    public class EasPaymentService : IEasPaymentService
    {
        private readonly IRepository<PaymentTypes> _paymentTypesRepository;

        public EasPaymentService(IRepository<PaymentTypes> paymentTypesRepository)
        {
            _paymentTypesRepository = paymentTypesRepository;
        }

        public List<PaymentTypes> GetAllPaymentTypes()
        {
            var query = _paymentTypesRepository.TableNoTracking.OrderBy(s => s.PaymentId).ThenBy(s => s.PaymentName);
            var paymentTypes = query.ToList();
            return paymentTypes;
        }
    }
}