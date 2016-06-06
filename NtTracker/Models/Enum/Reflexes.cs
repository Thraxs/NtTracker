using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Shared;

namespace NtTracker.Models
{
    public enum Reflexes
    {
        [Display(Name = "Reflexes_Normal", ResourceType = typeof(SharedStrings))]
        Normal = 2,
        [Display(Name = "Reflexes_Irregular", ResourceType = typeof(SharedStrings))]
        Irregular = 1,
        [Display(Name = "Reflexes_NoReaction", ResourceType = typeof(SharedStrings))]
        NoReaction = 0
    }
}