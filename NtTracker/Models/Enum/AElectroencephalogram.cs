using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Shared;

namespace NtTracker.Models
{
    public enum AElectroencephalogram
    {
        [Display(Name = "AEEG_Continuous", ResourceType = typeof(SharedStrings))]
        Continuous,
        [Display(Name = "AEEG_Discontinuous", ResourceType = typeof(SharedStrings))]
        Discontinuous,
        [Display(Name = "AEEG_BurstSupression", ResourceType = typeof(SharedStrings))]
        BurstSupression,
        [Display(Name = "AEEG_LowVoltage", ResourceType = typeof(SharedStrings))]
        LowVoltage,
        [Display(Name = "AEEG_Flat", ResourceType = typeof(SharedStrings))]
        Flat,
        [Display(Name = "AEEG_Convulsion", ResourceType = typeof(SharedStrings))]
        Convulsion
    }
}