using ESFA.DC.EAS1819.Tests.Base.Builders;
using ESFA.DC.EAS1819.ValidationService.Validators;

namespace ESFA.DC.EAS1819.ValidationService.Test.Validators.BusinessValidator
{
    using System;
    using System.Collections.Generic;
    using ESFA.DC.DateTimeProvider.Interface;
    using ESFA.DC.EAS1819.EF;
    using ESFA.DC.ReferenceData.FCS.Model;
    using Moq;

    public class BusinessValidatorBase
    {
        public BusinessRulesValidator _validator;
        public Mock<IDateTimeProvider> dateTimeProviderMock;
        public List<PaymentTypes> paymentTypes;
        public List<FundingLineContractTypeMapping> _fundingLineContractTypeMappings;
        public List<ContractAllocation> _contractAllocations;

        public BusinessValidatorBase()
        {
            dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _fundingLineContractTypeMappings = new FundingLineContractTypeMappingsBuilder().Build();
            _contractAllocations = new ContractAllocationsBuilder().Build();
            paymentTypes = new List<PaymentTypes>()
            {
                new PaymentTypes()
                {
                    FundingLine = new FundingLine(){ Id = 1, Name = "FundingLine"},
                    AdjustmentType = new AdjustmentType() { Id = 1, Name = "AdjustmentType" }
                },
                new PaymentTypes()
                {
                    FundingLine = new FundingLine(){ Id = 2, Name = "Funding-123+.Line"},
                    AdjustmentType = new AdjustmentType() { Id = 1, Name = "Adjustment-123+.Type" }
                }
            };
        }
    }
}
