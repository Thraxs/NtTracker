using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Patient;

namespace NtTracker.Models
{
    public enum PatientSex
    {
        [Display(Name = "PatientSex_Male", ResourceType = typeof(Strings))]
        Male,
        [Display(Name = "PatientSex_Female", ResourceType = typeof(Strings))]
        Female
    }
}