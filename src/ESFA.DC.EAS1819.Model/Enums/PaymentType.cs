using System.ComponentModel;

namespace ESFA.DC.EAS1819.Model.Enums
{
    public enum PaymentType
    {
        [Description("Excess Learning Support")]
        ExcessLearningSupport,

        [Description("Exceptional Learning Support")]
        ExceptionalLearningSupport,

        [Description("Audit Adjustments")]
        AuditAdjustments,

        [Description("Authorised Claims")]
        AuthorisedClaims,

        [Description("Learner Support")]
        LearnerSupport,

        [Description("Vulnerable Bursary")]
        VulnerableBursary,

        [Description("Discretionary Bursary")]
        DiscretionaryBursary,

        [Description("Free Meals")]
        FreeMeals,

        [Description("Area Costs Audit Adjustments")]
        AreaCostsAuditAdjustments,

        [Description("Excess Learning or Learner Support")]
        ExcessLearningOrLearnerSupport
    }
}