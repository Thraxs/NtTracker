using System;
using System.ComponentModel.DataAnnotations;

namespace NtTracker.Models
{
    public class Operation
    {
        ///
        /// Properties
        ///

        [ScaffoldColumn(false)]
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        [Required]
        public OperationType Action { get; set; }

        [Required]
        [StringLength(100)]
        public string OperationData { get; set; }

        ///
        /// Relationships
        /// 

        public int? UserId { get; set; }
        public virtual UserAccount User { get; set; }

        public int? PatientId { get; set; }
        public virtual Patient Patient { get; set; }
    }
}