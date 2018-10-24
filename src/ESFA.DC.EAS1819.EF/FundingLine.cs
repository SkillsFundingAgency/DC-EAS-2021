namespace ESFA.DC.EAS1819.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FundingLine : BaseEntity
    {
        private ICollection<FundingLineContractTypeMapping> _fundingLineContractTypeMappings;

        public FundingLine()
        {
            PaymentTypes = new List<PaymentTypes>();
            ContractTypes = new List<ContractType>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<PaymentTypes> PaymentTypes { get; set; }

        public virtual ICollection<ContractType> ContractTypes { get; set; }

        public virtual ICollection<FundingLineContractTypeMapping> FundingLineContractTypeMappings
        {
            get { return _fundingLineContractTypeMappings ?? (_fundingLineContractTypeMappings = new List<FundingLineContractTypeMapping>()); }
            protected set { _fundingLineContractTypeMappings = value; }
        }
    }
}
