using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Patient;

namespace NtTracker.Models
{
    public enum PatientStatus
    {
        [Display(Name = "PatientStatus_Normal", Description = "PatientStatus_NormalDesc", ResourceType = typeof(Strings))]
        Normal,
        [Display(Name = "PatientStatus_NbrSurveillance", Description = "PatientStatus_NbrSurveillanceDesc", ResourceType = typeof(Strings))]
        NbrSurveillance,
        [Display(Name = "PatientStatus_Monitoring", Description = "PatientStatus_MonitoringDesc", ResourceType = typeof(Strings))]
        Monitoring,
        [Display(Name = "PatientStatus_Hypothermia", Description = "PatientStatus_HypothermiaDesc", ResourceType = typeof(Strings))]
        Hypothermia,
        [Display(Name = "PatientStatus_Closed", Description = "PatientStatus_ClosedDesc", ResourceType = typeof(Strings))]
        Closed
    }
}