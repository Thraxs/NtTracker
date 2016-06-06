using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Shared;

namespace NtTracker.Models
{
    public enum TimeSlot
    {
        [Display(Name = "TimeSlot_6h", ResourceType = typeof(SharedStrings))]
        H6,
        [Display(Name = "TimeSlot_24h", ResourceType = typeof(SharedStrings))]
        H24,
        [Display(Name = "TimeSlot_48h", ResourceType = typeof(SharedStrings))]
        H48,
        [Display(Name = "TimeSlot_72h", ResourceType = typeof(SharedStrings))]
        H72,
        [Display(Name = "TimeSlot_5_7d", ResourceType = typeof(SharedStrings))]
        D57,
        [Display(Name = "TimeSlot_Discharge", ResourceType = typeof(SharedStrings))]
        Discharge
    }
}