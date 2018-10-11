using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.DataService.Interface;

namespace ESFA.DC.EAS1819.DataService
{
    // TODO: Need to refactor this once DCFS decides the architecture for the FundingLine table as its duplicated at the moment.
   public class FundingLineContractTypeMappingDataService : IFundingLineContractTypeMappingDataService
    {
        List<FundingLineContractTypeMapping> _mapping;

        public FundingLineContractTypeMappingDataService()
        {
            _mapping = new List<FundingLineContractTypeMapping>()
            {
                new FundingLineContractTypeMapping { FundingLine = "16-18 Apprenticeships", CongractTypeRequired = "APPS1819" },
                new FundingLineContractTypeMapping { FundingLine = "19-23 Apprenticeships", CongractTypeRequired = "APPS1819" },
                new FundingLineContractTypeMapping { FundingLine = "24+ Apprenticeships", CongractTypeRequired = "APPS1819" },
                new FundingLineContractTypeMapping { FundingLine = "16-18 Trailblazer Apprenticeships", CongractTypeRequired = "APPS1819" },
                new FundingLineContractTypeMapping { FundingLine = "19-23 Trailblazer Apprenticeships", CongractTypeRequired = "APPS1819" },
                new FundingLineContractTypeMapping { FundingLine = "24+ Trailblazer Apprenticeships", CongractTypeRequired = "APPS1819" },
                new FundingLineContractTypeMapping { FundingLine = "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)", CongractTypeRequired = "APPS1819" },
                new FundingLineContractTypeMapping { FundingLine = "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)", CongractTypeRequired = "APPS1819" },
                new FundingLineContractTypeMapping { FundingLine = "16-18 Apprenticeship (From May 2017) Levy Contract", CongractTypeRequired = "LEVY1799" },
                new FundingLineContractTypeMapping { FundingLine = "19+ Apprenticeship (From May 2017) Levy Contract", CongractTypeRequired = "LEVY1799" },
                new FundingLineContractTypeMapping { FundingLine = "16-18 Apprenticeship Non-Levy Contract (procured)", CongractTypeRequired = "16-18NLAP2018" },
                new FundingLineContractTypeMapping { FundingLine = "19+ Apprenticeship Non-Levy Contract (procured)", CongractTypeRequired = "ANLAP2018" },
                new FundingLineContractTypeMapping { FundingLine = "16-18 Traineeships", CongractTypeRequired = "16-18TRN" },
                new FundingLineContractTypeMapping { FundingLine = "16-19 Traineeships Bursary", CongractTypeRequired = "16-18TRN" },
                new FundingLineContractTypeMapping { FundingLine = "AEB - Other Learning (non-procured)", CongractTypeRequired = "AEBC or AEBTO-TOL" },
                new FundingLineContractTypeMapping { FundingLine = "19-24 Traineeships (non-procured)", CongractTypeRequired = "AEBC or AEBTO-TOL" },
                new FundingLineContractTypeMapping { FundingLine = "AEB - Other Learning (procured from Nov 2017)", CongractTypeRequired = "AEB-TOL" },
                new FundingLineContractTypeMapping { FundingLine = "19-24 Traineeships (procured from Nov 2017)", CongractTypeRequired = "AEB-TOL" },
                new FundingLineContractTypeMapping { FundingLine = "Advanced Learner Loans Bursary", CongractTypeRequired = "ALLB or ALLBC" }
            };
        }

        public List<string> GetContractTypesRequired(string fundingLine)
        {
            var list = _mapping.Where(x => x.FundingLine.Equals(fundingLine)).Select(x => x.CongractTypeRequired).ToList();
            return list;
        }
    }

    public class FundingLineContractTypeMapping
    {
        public string FundingLine { get; set; }

        public string CongractTypeRequired { get; set; }
    }
}
