using System;
using System.Collections.Generic;
using ESFA.DC.EAS1819.EF;

namespace ESFA.DC.EAS1819.DataService.Interface
{
    public interface IEasPaymentService
    {
        List<PaymentTypes> GetAllPaymentTypes();
    }
}
