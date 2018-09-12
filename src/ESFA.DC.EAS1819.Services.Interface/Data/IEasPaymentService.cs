using System;
using System.Collections.Generic;
using ESFA.DC.EAS1819.Data;

namespace ESFA.DC.EAS1819.Services.Interface.Data
{
    public interface IEasPaymentService
    {
        List<PaymentTypes> GetAllPaymentTypes();
    }
}
