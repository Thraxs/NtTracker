using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Monitoring;

namespace NtTracker.Models
{
    public enum DevelopmentReflexes
    {
        [Display(Name = "DevelopmentReflexes_Present", ResourceType = typeof(Strings))]
        Present = 2,
        [Display(Name = "DevelopmentReflexes_Absent", ResourceType = typeof(Strings))]
        Absent = 1,
        [Display(Name = "DevelopmentReflexes_Extreme", ResourceType = typeof(Strings))]
        Extreme = 0
    }
}