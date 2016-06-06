using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Shared;

namespace NtTracker.Models
{
    public enum Electroencephalogram
    {
        [Display(Name = "EEG_Normal", ResourceType = typeof(SharedStrings))]
        Normal,
        [Display(Name = "EEG_LowVoltage", ResourceType = typeof(SharedStrings))]
        LowVoltage,
        [Display(Name = "EEG_Slow", ResourceType = typeof(SharedStrings))]
        Slow
    }
}