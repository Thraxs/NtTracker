using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Shared;

namespace NtTracker.Models
{
    public enum Tone
    {
        [Display(Name = "Tone_Good", ResourceType = typeof(SharedStrings))]
        Good = 2,
        [Display(Name = "Tone_Irregular", ResourceType = typeof(SharedStrings))]
        Irregular = 1,
        [Display(Name = "Tone_Low", ResourceType = typeof(SharedStrings))]
        Low = 0
    }
}