using ESFA.DC.EAS2021.EF;

namespace ESFA.DC.EAS.Tests.Base.Builders
{
    using System.Collections.Generic;

    public class PaymentTypesBuilder
    {
        public List<PaymentType> GetPaymentTypeList()
        {
            var paymentTypes = new List<PaymentType>()
            {
                new PaymentType()
                {
                    FundingLine = new FundingLine { Id = 1, Name = "16-18 Apprenticeships" },
                    AdjustmentType = new AdjustmentType() { Id = 1, Name = "Excess Learning Support" }
                },
                new PaymentType()
                {
                    FundingLine = new FundingLine { Id = 2, Name = "24+ Apprenticeships" },
                    AdjustmentType = new AdjustmentType() { Id = 1, Name = "Authorised Claims" }
                }
            };

            return paymentTypes;
        }
    }
}
