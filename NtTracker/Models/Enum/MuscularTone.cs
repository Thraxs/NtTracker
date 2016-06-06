using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Monitoring;

namespace NtTracker.Models
{
    public enum MuscularTone
    {
        [Display(Name = "MuscularTone_Normal", ResourceType = typeof(Strings))]
        Normal = 3,
        [Display(Name = "MuscularTone_Hypotonia", ResourceType = typeof(Strings))]
        Hypotonia = 2,
        [Display(Name = "MuscularTone_Hypertonia", ResourceType = typeof(Strings))]
        Hypertonia = 1,
        [Display(Name = "MuscularTone_Both", ResourceType = typeof(Strings))]
        Both = 0
    }
}