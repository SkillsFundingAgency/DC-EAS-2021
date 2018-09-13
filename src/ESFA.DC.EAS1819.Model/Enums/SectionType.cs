using System.ComponentModel;

namespace ESFA.DC.EAS1819.Model.Enums
{
    public enum SectionType
    {
        [Description("Apprenticeships for starts before 1 May 2017")]
        ApprenticeshipBeforeMay,

        [Description("Apprenticeships for starts on or after 1 May 2017")]
        ApprenticeshipAfterMay,

        [Description("16-18 Traineeships")]
        SfaFundedTraineeship,

        [Description("Adult Education -Traineeships and Other Learning")]
        AdultEducation,

        [Description("Advanced Learner Loans")]
        AdvancedLearnerLoans,

        [Description("16-18 Levy contracted Apprenticeships")]
        LevyContractedApprenticeships,

        [Description("16-18 Non-levy contracted Apprenticeships")]
        NonLevyContractedApprenticeships,

        [Description("Adult Levy contracted Apprenticeships")]
        AdultLevyContractedApprenticeships,

        [Description("Adult Non-levy contracted Apprenticeships")]
        AdultNonLevyContractedApprenticeships,

        [Description("Adult Education - Non-procured delivery")]
        AdultEducationNonPrcDelivery,

        [Description("Adult Education - Procured delivery from 1 Nov 2017")]
        AdultEducationPrcDelFrom1Nov2017,

        [Description("16-18 Non-levy contracted Apprenticeships - Non-procured delivery")]
        NonLevyContractedApprenticeshipsBfr31Dec,

        [Description("16-18 Non-levy contracted Apprenticeships - Procured delivery")]
        NonLevyContractedApprenticeshipsFrom1Jan,

        [Description("Adult Non-levy contracted Apprenticeships - Non-procured delivery")]
        AdultNonLevyContractedApprenticeshipsBfr31Dec,

        [Description("Adult Non-levy contracted Apprenticeships - Procured delivery")]
        AdultNonLevyContractedApprenticeshipsFrom1Jan,

        [Description("16-18 Non-levy contracted Apprenticeships for starts from 1 May 2017 to 31 Dec 2017")]
        NonLevyContractedApprenticeshipsBfr31DecDeprecated,

        [Description("Adult Non-levy contracted Apprenticeships for starts from 1 May 2017 to 31 Dec 2017")]
        AdultNonLevyContractedApprenticeshipsBfr31DecDeprecated,
    }
}