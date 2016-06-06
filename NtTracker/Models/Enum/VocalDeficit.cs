using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Monitoring;

namespace NtTracker.Models
{
    public enum VocalDeficit
    {
        [Display(Name = "VocalDeficit_No", ResourceType = typeof(Strings))]
        No = 2,
        [Display(Name = "VocalDeficit_Babbling", ResourceType = typeof(Strings))]
        Babbling = 1,
        [Display(Name = "VocalDeficit_Snarl", ResourceType = typeof(Strings))]
        Snarl = 0
    }
}