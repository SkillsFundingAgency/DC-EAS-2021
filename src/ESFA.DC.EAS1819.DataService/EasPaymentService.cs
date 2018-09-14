namespace ESESFA.DC.EAS1819.DataService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ESFA.DC.EAS1819.DataService.Interface;
    using ESFA.DC.EAS1819.EF;

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