using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.EF;

namespace ESFA.DC.EAS1819.Tests.Base.Builders
{
    public class PaymentTypesBuilder
    {
        public List<PaymentTypes> GetPaymentTypeList()
        {
            var paymentTypes = new List<PaymentTypes>()
            {
                new PaymentTypes()
                {
                    FundingLine = "16-18 Apprenticeships",
                    AdjustmentType = "Excess Learning Support"
                },
                new PaymentTypes()
                {
                    FundingLine = "24+ Apprenticeships",
                    AdjustmentType = "Authorised Claims"
                }
            };
            return paymentTypes;
        }
    }
}
