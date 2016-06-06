using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Monitoring;

namespace NtTracker.Models
{
    public enum VisualDeficit
    {
        [Display(Name = "VisualDeficit_No", ResourceType = typeof(Strings))]
        No = 2,
        [Display(Name = "VisualDeficit_Decrease", ResourceType = typeof(Strings))]
        Decrease = 1,
        [Display(Name = "VisualDeficit_Blindness", ResourceType = typeof(Strings))]
        Blindness = 0
    }
}