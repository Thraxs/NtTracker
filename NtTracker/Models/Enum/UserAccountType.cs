using System.ComponentModel.DataAnnotations;
using NtTracker.Resources.UserAccount;

namespace NtTracker.Models
{
    public enum UserAccountType
    {
        [Display(Name = "Type_Normal", ResourceType = typeof(Strings))]
        Normal,
        [Display(Name = "Type_Admin", ResourceType = typeof(Strings))]
        Admin
    }
}