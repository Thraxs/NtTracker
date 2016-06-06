using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Patient;

namespace NtTracker.Models
{
    public enum BirthType
    {
        [Display(Name = "BirthType_Eutocic", ResourceType = typeof(Strings))]
        Eutocic,
        [Display(Name = "BirthType_Cesarean", ResourceType = typeof(Strings))]
        Cesarean,
        [Display(Name = "BirthType_Forceps", ResourceType = typeof(Strings))]
        Forceps,
        [Display(Name = "BirthType_Ventouse", ResourceType = typeof(Strings))]
        Ventouse,
        [Display(Name = "BirthType_Spatulas", ResourceType = typeof(Strings))]
        Spatulas
    }
}