using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Hypothermia;

namespace NtTracker.Models
{
    public enum CerebralResonance
    {
        [Display(Name = "CMRI_Normal", ResourceType = typeof(Strings))]
        Normal,
        [Display(Name = "CMRI_BaseGG", ResourceType = typeof(Strings))]
        BaseGg,
        [Display(Name = "CMRI_ThalamusInjury", ResourceType = typeof(Strings))]
        ThalamusInjury,
        [Display(Name = "CMRI_CorpusCallosumInjury", ResourceType = typeof(Strings))]
        CorpusCallosumInjury,
        [Display(Name = "CMRI_CerebralCortexInjury", ResourceType = typeof(Strings))]
        CerebralCortexInjury
    }
}