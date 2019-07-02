using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Tests.Base.Builders;
using ESFA.DC.EAS1819.ValidationService.Validators;
using ESFA.DC.ReferenceData.FCS.Model;
using Moq;

namespace ESFA.DC.EAS1819.ValidationService.Test.Validators.BusinessValidator
{
    public abstract class BusinessValidatorBase
    {
        protected BusinessRulesValidator _validator;
        protected Mock<IDateTimeProvider> dateTimeProviderMock;
        protected List<PaymentType> paymentTypes;
        protected List<FundingLineContractTypeMapping> _fundingLineContractTypeMappings;
        protected List<ContractAllocation> _contractAllocations;

        protected BusinessValidatorBase()
        {
            dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _fundingLineContractTypeMappings = new FundingLineContractTypeMappingsBuilder().Build();
            _contractAllocations = new ContractAllocationsBuilder().Build();
            paymentTypes = new List<PaymentType>
            {
                new PaymentType
                {
                    FundingLine = new FundingLine { Id = 1, Name = "FundingLine" },
                    AdjustmentType = new AdjustmentType { Id = 1, Name = "AdjustmentType" }
                },
                new PaymentType
                {
                    FundingLine = new FundingLine { Id = 2, Name = "Funding-123+.Line" },
                    AdjustmentType = new AdjustmentType { Id = 1, Name = "Adjustment-123+.Type" }
                }
            };
        }
    }
}
