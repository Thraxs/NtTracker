using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NtTracker.Models
{
    public class Patient
    {
        ///
        /// Properties
        ///

        // --- Administrative data ---

        /// <summary>
        /// NtTracker patient ID.
        /// </summary>
        [ScaffoldColumn(false)]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Clinic History Number.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Nhc { get; set; }

        /// <summary>
        /// Andalusia Unique Health History Number.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Nuhsa { get; set; }

        /// <summary>
        /// Name of the user that registered this patient.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string RegistrantName { get; set; }

        // --- Personal data ---

        /// <summary>
        /// Patient name.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Patient surnames.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Surnames { get; set; }

        /// <summary>
        /// Patient sex.
        /// </summary>
        [Required]
        public PatientSex Sex { get; set; }

        /// <summary>
        /// Patient birthdate.
        /// </summary>
        [Required]
        public DateTime BirthDate { get; set; }

        // --- Perinatal data ---

        /// <summary>
        /// Patient birth type.
        /// </summary>
        [Required]
        public BirthType BirthType { get; set; }

        /// <summary>
        /// Patient pH at birth
        /// </summary>
        [Required]
        [Range(0, 14)]
        public double Ph { get; set; }

        /// <summary>
        /// Patient Apgar at birth.
        /// </summary>
        [Required]
        [Range(0, 10)]
        public int Apgar { get; set; }

        /// <summary>
        /// Patient weight at birth.
        /// </summary>
        [Required]
        [Range(1, 20000)]
        public double Weight { get; set; }

        /// <summary>
        /// Type of CPR performed on the patient.
        /// </summary>
        public CprType? CprType { get; set; }

        // --- Consciousness ---

        /// <summary>
        /// Patient has lethatgy.
        /// </summary>
        public bool Lethargy { get; set; }

        /// <summary>
        /// Patient has stupor.
        /// </summary>
        public bool Stupor { get; set; }

        /// <summary>
        /// Patient is in a coma.
        /// </summary>
        public bool Coma { get; set; }

        // --- Other ---

        /// <summary>
        /// Patient has altered tone.
        /// </summary>
        public bool AlteredTone { get; set; }

        /// <summary>
        /// Patient has altered reflexes.
        /// </summary>
        public bool AlteredReflexes { get; set; }

        /// <summary>
        /// Patient has altered suction.
        /// </summary>
        public bool AlteredSuction { get; set; }

        /// <summary>
        /// Patient has convulsions.
        /// </summary>
        public bool Convulsion { get; set; }

        // --- Current status ---

        /// <summary>
        /// Current patient status.
        /// </summary>
        public PatientStatus PatientStatus { get; set; }

        /// <summary>
        /// Status of the patient if his tracking is closed.
        /// </summary>
        public PatientStatus LastStatus { get; set; }

        ///
        /// Relationships
        /// 

        /// <summary>
        /// User that registered this patient. May be null if user 
        /// was deleted from the system.
        /// </summary>
        public int? RegistrantId { get; set; }
        public virtual UserAccount Registrant { get; set; }

        /// <summary>
        /// NBR surveillances of this patient.
        /// </summary>
        public virtual ICollection<NbrSurveillance> NbrSurveillances { get; set; }

        /// <summary>
        /// Monitorings of this patient.
        /// </summary>
        public virtual ICollection<Monitoring> Monitorings { get; set; }

        /// <summary>
        /// Related hypothermia monitorizations of this patient.
        /// </summary>
        public virtual ICollection<Hypothermia> Hypothermias { get; set; }

        /// <summary>
        /// Operations performed in the system that affect this patient.
        /// </summary>
        public virtual ICollection<Operation> Operations { get; set; }
    }
}