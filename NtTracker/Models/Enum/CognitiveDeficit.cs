using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Monitoring;

namespace NtTracker.Models
{
    public enum CognitiveDeficit
    {
        [Display(Name = "CognitiveDeficit_No", ResourceType = typeof(Strings))]
        No = 4,
        [Display(Name = "CognitiveDeficit_Mild", ResourceType = typeof(Strings))]
        Mild = 3,
        [Display(Name = "CognitiveDeficit_Moderate", ResourceType = typeof(Strings))]
        Moderate = 2,
        [Display(Name = "CognitiveDeficit_Severe", ResourceType = typeof(Strings))]
        Severe = 1,
        [Display(Name = "CognitiveDeficit_Deep", ResourceType = typeof(Strings))]
        Deep = 0
    }
}