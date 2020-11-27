using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.Tests.Base.Builders;
using ESFA.DC.EAS.ValidationService.Validators;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.ReferenceData.FCS.Model;
using Moq;

namespace ESFA.DC.EAS.ValidationService.Test.Validators.BusinessValidator
{
    public abstract class BusinessValidatorBase
    {
        protected BusinessRulesValidator _validator;
        protected Mock<IDateTimeProvider> dateTimeProviderMock;
        protected List<PaymentType> paymentTypes;
        protected List<FundingLineContractTypeMapping> _fundingLineContractTypeMappings;
        protected List<ContractAllocation> _contractAllocations;
        protected IReadOnlyDictionary<string, IEnumerable<DevolvedContract>> _devolvedContracts;
        protected IReadOnlyDictionary<int, string> _sofCodeDictionary;


        protected BusinessValidatorBase()
        {
            dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _fundingLineContractTypeMappings = new FundingLineContractTypeMappingsBuilder().Build();
            _contractAllocations = new ContractAllocationsBuilder().Build();
            _devolvedContracts = new DevolvedContractsBuilder().Build();
            _sofCodeDictionary = new McaSofDictionaryBuilder().Build();
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
