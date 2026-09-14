using System.ComponentModel;

namespace ExerciseApp.Model
{
    public enum InsuranceType {
        [Description("Fully Comprehensive")]
        FullyComprehensive,
        [Description("Third Party Fire & Theft")]
        ThirdPartyFireAndTheft,
        [Description("Third Party Only")]
        ThirdPartyOnly
    }
}


