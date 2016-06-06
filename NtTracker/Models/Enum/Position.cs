using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Shared;

namespace NtTracker.Models
{
    public enum Position
    {
        [Display(Name = "Position_Good", ResourceType = typeof(SharedStrings))]
        Good = 2,
        [Display(Name = "Position_Irregular", ResourceType = typeof(SharedStrings))]
        Irregular = 1,
        [Display(Name = "Position_Bad", ResourceType = typeof(SharedStrings))]
        Bad = 0
    }
}