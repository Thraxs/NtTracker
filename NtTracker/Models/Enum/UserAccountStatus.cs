using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.UserAccount;

namespace NtTracker.Models
{
    public enum UserAccountStatus
    {
        [Display(Name = "Status_Normal", ResourceType = typeof(Strings))]
        Normal,
        [Display(Name = "Status_Locked", ResourceType = typeof(Strings))]
        Locked
    }
}