using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using NtTracker.Models;
using NtTracker.Resources.Shared;
using NtTracker.Resources.UserAccount;

namespace NtTracker.ViewModels
{
    public class UserAccountViewModel : AbstractViewModel
    {
        ///
        /// Mandatory properties
        //

        private string _username;

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Username", ResourceType = typeof(Strings))]
        public string UserName
        {
            get { return _username; }
            set
            {
                //Since LDAP usernames are case insensitive, always make username lowercase
                _username = value.ToLower(Thread.CurrentThread.CurrentCulture);
            }
        }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Strings))]
        public string Password { get; set; }

        ///
        /// Readonly properties
        ///

        [Display(Name = "Id", ResourceType = typeof(Strings))]
        public int Id { get; }

        [Display(Name = "IsAdmin", ResourceType = typeof(Strings))]
        public bool IsAdmin { get; }

        [Display(Name = "RegistrationDate", ResourceType = typeof(Strings))]
        public DateTime RegistrationDate { get; }

        [Display(Name = "LastLogin", ResourceType = typeof(Strings))]
        public DateTime? LastLogin { get; }

        [Display(Name = "LastLoginInfo", ResourceType = typeof(Strings))]
        public string LastLoginInfo { get; }
    
        [Display(Name = "Status_Locked", ResourceType = typeof(Strings))]
        public bool IsLocked { get; }

        [Display(Name = "UnlockDate", ResourceType = typeof(Strings))]
        public DateTime? UnlockDate { get; }

        [Display(Name = "FailedAttempts", ResourceType = typeof(Strings))]
        public int FailedLoginAttempts { get; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public UserAccountViewModel() { }

        /// <summary>
        /// Creates a new UserAccount viewmodel with the data
        /// from the given UserAccount model entity.
        /// </summary>
        /// <param name="account">Account model entity to get values from.</param>
        public UserAccountViewModel(UserAccount account)
        {
            UserName = account.UserName;
            Id = account.Id;
            IsAdmin = account.IsAdmin;
            RegistrationDate = account.RegistrationDate;
            LastLogin = account.LastLogin;
            LastLoginInfo = account.LastLoginInfo;
            IsLocked = account.IsLocked;
            UnlockDate = account.UnlockDate;
            FailedLoginAttempts = account.FailedLoginAttempts;
        }
    }
}