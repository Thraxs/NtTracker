using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NtTracker.Models
{
    public class UserAccount
    {
        ///
        /// Properties
        ///

        [ScaffoldColumn(false)]
        [Key]
        public int Id { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }

        public DateTime? LastLogin { get; set; }

        [StringLength(300)]
        public string LastLoginInfo { get; set; }

        [Required]
        public bool IsLocked { get; set; }

        public DateTime? UnlockDate { get; set; }

        [Required]
        public int FailedLoginAttempts { get; set; }

        ///
        /// Relationships
        /// 

        /// <summary>
        /// Patients registered by this user.
        /// </summary>
        public virtual ICollection<Patient> PatientsRegistered { get; set; }

        /// <summary>
        /// Operations performed by this user.
        /// </summary>
        public virtual ICollection<Operation> Operations { get; set; }
    }
}