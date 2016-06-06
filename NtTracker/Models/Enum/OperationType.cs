using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.Operation;

namespace NtTracker.Models
{
    public enum OperationType
    {
        [Display(Name = "Type_AccountRegister", Description = "Type_AccountRegister_Desc", ResourceType = typeof(Strings))]
        Register,
        [Display(Name = "Type_AccountLogin", Description = "Type_AccountLogin_Desc", ResourceType = typeof(Strings))]
        Login,
        [Display(Name = "Type_AccountLock", Description = "Type_AccountLock_Desc", ResourceType = typeof(Strings))]
        AccountLocked,
        [Display(Name = "Type_AccountManualLock", Description = "Type_AccountManualLock_Desc", ResourceType = typeof(Strings))]
        AccountManualLock,
        [Display(Name = "Type_AccountManualUnlock", Description = "Type_AccountManualUnlock_Desc", ResourceType = typeof(Strings))]
        AccountManualUnlock,
        [Display(Name = "Type_AccountDelete", Description = "Type_AccountDelete_Desc", ResourceType = typeof(Strings))]
        AccountDelete,
        [Display(Name = "Type_PatientCreate", Description = "Type_PatientCreate_Desc", ResourceType = typeof(Strings))]
        PatientCreate,
        [Display(Name = "Type_PatientUpdate", Description = "Type_PatientUpdate_Desc", ResourceType = typeof(Strings))]
        PatientUpdate,
        [Display(Name = "Type_PatientDelete", Description = "Type_PatientDelete_Desc", ResourceType = typeof(Strings))]
        PatientDelete,
        [Display(Name = "Type_PatientClose", Description = "Type_PatientClose_Desc", ResourceType = typeof(Strings))]
        PatientClose,
        [Display(Name = "Type_PatientOpen", Description = "Type_PatientOpen_Desc", ResourceType = typeof(Strings))]
        PatientOpen,
        [Display(Name = "Type_NbrSurveillanceCreate", Description = "Type_NbrSurveillanceCreate_Desc", ResourceType = typeof(Strings))]
        NbrSurveillanceCreate,
        [Display(Name = "Type_NbrSurveillanceUpdate", Description = "Type_NbrSurveillanceUpdate_Desc", ResourceType = typeof(Strings))]
        NbrSurveillanceUpdate,
        [Display(Name = "Type_NbrSurveillanceDelete", Description = "Type_NbrSurveillanceDelete_Desc", ResourceType = typeof(Strings))]
        NbrSurveillanceDelete,
        [Display(Name = "Type_MonitoringCreate", Description = "Type_MonitoringCreate_Desc", ResourceType = typeof(Strings))]
        MonitoringCreate,
        [Display(Name = "Type_MonitoringUpdate", Description = "Type_MonitoringUpdate_Desc", ResourceType = typeof(Strings))]
        MonitoringUpdate,
        [Display(Name = "Type_MonitoringDelete", Description = "Type_MonitoringDelete_Desc", ResourceType = typeof(Strings))]
        MonitoringDelete,
        [Display(Name = "Type_HypothermiaCreate", Description = "Type_HypothermiaCreate_Desc", ResourceType = typeof(Strings))]
        HypothermiaCreate,
        [Display(Name = "Type_HypothermiaUpdate", Description = "Type_HypothermiaUpdate_Desc", ResourceType = typeof(Strings))]
        HypothermiaUpdate,
        [Display(Name = "Type_HypothermiaDelete", Description = "Type_HypothermiaDelete_Desc", ResourceType = typeof(Strings))]
        HypothermiaDelete
    }
}