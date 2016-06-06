using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.NbrSurveillance;

namespace NtTracker.Models
{
    public enum TransfontanellarUltrasound
    {
        [Display(Name = "TfUs_Normal", ResourceType = typeof(Strings))]
        Normal,
        [Display(Name = "TfUs_Edema", ResourceType = typeof(Strings))]
        Edema,
        [Display(Name = "TfUs_ThalamusInjury", ResourceType = typeof(Strings))]
        ThalamusInjury,
        [Display(Name = "TfUs_BaseGG", ResourceType = typeof(Strings))]
        BaseGg
    }
}