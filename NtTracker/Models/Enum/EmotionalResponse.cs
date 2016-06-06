using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Monitoring;

namespace NtTracker.Models
{
    public enum EmotionalResponse
    {
        [Display(Name = "EmotionalResponse_Pleasure", ResourceType = typeof(Strings))]
        Pleasure = 2,
        [Display(Name = "EmotionalResponse_Displeasure", ResourceType = typeof(Strings))]
        Displeasure = 1,
        [Display(Name = "EmotionalResponse_None", ResourceType = typeof(Strings))]
        None = 0
    }
}