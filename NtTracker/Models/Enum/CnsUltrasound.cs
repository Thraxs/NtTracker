using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Hypothermia;

namespace NtTracker.Models
{
    public enum CnsUltrasound
    {
        [Display(Name = "CnsUs_Normal", ResourceType = typeof(Strings))]
        Normal,
        [Display(Name = "CnsUs_Edema", ResourceType = typeof(Strings))]
        Edema,
        [Display(Name = "CnsUs_ThalamusInjury", ResourceType = typeof(Strings))]
        ThalamusInjury,
        [Display(Name = "CnsUs_BaseGG", ResourceType = typeof(Strings))]
        BaseGg
    }
}