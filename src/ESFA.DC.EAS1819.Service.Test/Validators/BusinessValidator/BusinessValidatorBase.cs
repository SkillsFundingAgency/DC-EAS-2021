using ESFA.DC.EAS1819.ValidationService.Validators;

namespace ESFA.DC.EAS1819.Service.Test.Validators.BusinessValidator
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
        public List<FundingLineContractMapping> _fundingLineContractMappings;
        public List<ContractAllocation> _contractAllocations;

        public BusinessValidatorBase()
        {
            dateTimeProviderMock = new Mock<IDateTimeProvider>();
            paymentTypes = new List<PaymentTypes>()
            {
                new PaymentTypes { AdjustmentType = "AdjustmentType", FundingLine = "FundingLine" },
                new PaymentTypes { AdjustmentType = "Adjustment-123+.Type", FundingLine = "Funding-123+.Line" }
            };
            _fundingLineContractMappings = new List<FundingLineContractMapping>()
            {
                new FundingLineContractMapping
                    { FundingLine = "FundingLine", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping
                    { FundingLine = "Funding-123+.Line", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping
                    { FundingLine = "16-18 Apprenticeships", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping
                    { FundingLine = "19-23 Apprenticeships", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping
                    { FundingLine = "24+ Apprenticeships", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping
                    { FundingLine = "19-24 Traineeships (procured from Nov 2017)", ContractTypeRequired = "AEB-TOL" },
                new FundingLineContractMapping
                    { FundingLine = "Advanced Learner Loans Bursary", ContractTypeRequired = "ALLB" }
            };

            _contractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation
                {
                    FundingStreamPeriodCode = "APPS1819", StartDate = new DateTime(2018, 01, 01),
                    EndDate = new DateTime(2019, 12, 01)
                },
                new ContractAllocation
                {
                    FundingStreamPeriodCode = "AEB-TOL", StartDate = new DateTime(2018, 01, 01),
                    EndDate = new DateTime(2019, 12, 01)
                },
            };
        }
    }
}
