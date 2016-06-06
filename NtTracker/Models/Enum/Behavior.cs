using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Shared;

namespace NtTracker.Models
{
    public enum Behavior
    {
        [Display(Name = "Behavior_Normal", ResourceType = typeof(SharedStrings))]
        Normal = 2,
        [Display(Name = "Behavior_Irregular", ResourceType = typeof(SharedStrings))]
        Irregular = 1,
        [Display(Name = "Behavior_NoResponse", ResourceType = typeof(SharedStrings))]
        NoResponse = 0
    }
}