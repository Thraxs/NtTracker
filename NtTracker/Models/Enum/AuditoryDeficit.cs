using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Monitoring;

namespace NtTracker.Models
{
    public enum AuditoryDeficit
    {
        [Display(Name = "AuditoryDeficit_No", ResourceType = typeof(Strings))]
        No = 2,
        [Display(Name = "AuditoryDeficit_Hypoacusia", ResourceType = typeof(Strings))]
        Hypoacusia = 1,
        [Display(Name = "AuditoryDeficit_Deafness", ResourceType = typeof(Strings))]
        Deafness = 0
    }
}