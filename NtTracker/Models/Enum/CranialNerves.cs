using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Shared;

namespace NtTracker.Models
{
    public enum CranialNerves
    {
        [Display(Name = "CranialNerves_Normal", ResourceType = typeof(SharedStrings))]
        Normal = 1,
        [Display(Name = "CranialNerves_Irregular", ResourceType = typeof(SharedStrings))]
        Irregular = 0
    }
}