using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.EF;

namespace ESFA.DC.EAS1819.DataService
{
    // TODO: Need to refactor this once DCFS decides the architecture for the FundingLine table as its duplicated at the moment.
   public class FundingLineContractTypeMappingDataService : IFundingLineContractTypeMappingDataService
    {
        List<FundingLineContractMapping> _mapping;

        public FundingLineContractTypeMappingDataService()
        {
            _mapping = new List<FundingLineContractMapping>()
            {
                new FundingLineContractMapping { FundingLine = "16-18 Apprenticeships", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping { FundingLine = "19-23 Apprenticeships", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping { FundingLine = "24+ Apprenticeships", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping { FundingLine = "16-18 Trailblazer Apprenticeships", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping { FundingLine = "19-23 Trailblazer Apprenticeships", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping { FundingLine = "24+ Trailblazer Apprenticeships", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping { FundingLine = "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping { FundingLine = "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping { FundingLine = "16-18 Apprenticeship (From May 2017) Levy Contract", ContractTypeRequired = "LEVY1799" },
                new FundingLineContractMapping { FundingLine = "19+ Apprenticeship (From May 2017) Levy Contract", ContractTypeRequired = "LEVY1799" },
                new FundingLineContractMapping { FundingLine = "16-18 Apprenticeship Non-Levy Contract (procured)", ContractTypeRequired = "16-18NLAP2018" },
                new FundingLineContractMapping { FundingLine = "19+ Apprenticeship Non-Levy Contract (procured)", ContractTypeRequired = "ANLAP2018" },
                new FundingLineContractMapping { FundingLine = "16-18 Traineeships", ContractTypeRequired = "16-18TRN" },
                new FundingLineContractMapping { FundingLine = "16-19 Traineeships Bursary", ContractTypeRequired = "16-18TRN" },
                new FundingLineContractMapping { FundingLine = "AEB - Other Learning (non-procured)", ContractTypeRequired = "AEBC or AEBTO-TOL" },
                new FundingLineContractMapping { FundingLine = "19-24 Traineeships (non-procured)", ContractTypeRequired = "AEBC or AEBTO-TOL" },
                new FundingLineContractMapping { FundingLine = "AEB - Other Learning (procured from Nov 2017)", ContractTypeRequired = "AEB-TOL" },
                new FundingLineContractMapping { FundingLine = "19-24 Traineeships (procured from Nov 2017)", ContractTypeRequired = "AEB-TOL" },
                new FundingLineContractMapping { FundingLine = "Advanced Learner Loans Bursary", ContractTypeRequired = "ALLB or ALLBC" }
            };
        }

        public List<FundingLineContractMapping> GetAllFundingLineContractTypeMappings()
        {
            //var list = _mapping.Where(x => x.FundingLine.Equals(fundingLine)).Select(x => x.ContractTypeRequired).ToList();
            //return list;
            return _mapping;
        }
    }
}
