using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Patient;

namespace NtTracker.Models
{
    public enum CprType
    {
        [Display(Name = "CprType_1", ResourceType = typeof(Strings))]
        Type1,
        [Display(Name = "CprType_2", ResourceType = typeof(Strings))]
        Type2,
        [Display(Name = "CprType_3", ResourceType = typeof(Strings))]
        Type3,
        [Display(Name = "CprType_4", ResourceType = typeof(Strings))]
        Type4,
        [Display(Name = "CprType_5", ResourceType = typeof(Strings))]
        Type5
    }
}