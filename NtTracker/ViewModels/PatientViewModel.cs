using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NtTracker.Models;
using NtTracker.Resources.Patient;
using NtTracker.Resources.Shared;

namespace NtTracker.ViewModels
{
    public class PatientViewModel : AbstractViewModel
    {
        ///
        /// Readonly properties
        ///

        [Display(Name = "IdDesc", ResourceType = typeof(Strings))]
        public int Id { get; set; }

        [Display(Name = "RegistrantName", ResourceType = typeof(Strings))]
        public string RegistrantName { get; set; }

        [Display(Name = "PatientStatus", ResourceType = typeof(Strings))]
        public PatientStatus PatientStatus { get; set; }

        [Display(Name = "LastStatus", ResourceType = typeof(Strings))]
        public PatientStatus LastStatus { get; set; }

        ///
        /// Mandatory properties
        ///

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [StringLength(20, MinimumLength = 1, ErrorMessageResourceName = "Validation_StringLength", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "NhcDesc", ResourceType = typeof(Strings))]
        public string Nhc { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [StringLength(20, MinimumLength = 1, ErrorMessageResourceName = "Validation_StringLength", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "NuhsaDesc", ResourceType = typeof(Strings))]
        public string Nuhsa { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [StringLength(100, MinimumLength = 1, ErrorMessageResourceName = "Validation_StringLength", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Name", ResourceType = typeof(Strings))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [StringLength(100, MinimumLength = 1, ErrorMessageResourceName = "Validation_StringLength", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Surnames", ResourceType = typeof(Strings))]
        public string Surnames { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "PatientSex", ResourceType = typeof(Strings))]
        public PatientSex Sex { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "BirthDate", ResourceType = typeof(Strings))]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "BirthType", ResourceType = typeof(Strings))]
        public BirthType BirthType { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Range(0, 14, ErrorMessageResourceName = "Validation_Range", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Ph", ResourceType = typeof(Strings))]
        public double Ph { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Range(0, 10, ErrorMessageResourceName = "Validation_Range", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Apgar", ResourceType = typeof(Strings))]
        public int Apgar { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Range(1, 20000, ErrorMessageResourceName = "Validation_Range", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Weight", ResourceType = typeof(Strings))]
        public double Weight { get; set; }

        [Display(Name = "CprType", ResourceType = typeof(Strings))]
        public CprType? CprType { get; set; }

        ///
        /// Optional properties
        ///

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Lethargy", ResourceType = typeof(Strings))]
        public bool Lethargy { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Stupor", ResourceType = typeof(Strings))]
        public bool Stupor { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Coma", ResourceType = typeof(Strings))]
        public bool Coma { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "AlteredTone", ResourceType = typeof(Strings))]
        public bool AlteredTone { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "AlteredReflexes", ResourceType = typeof(Strings))]
        public bool AlteredReflexes { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "AlteredSuction", ResourceType = typeof(Strings))]
        public bool AlteredSuction { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Convulsion", ResourceType = typeof(SharedStrings))]
        public bool Convulsion { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public PatientViewModel() { }

        /// <summary>
        /// Creates a new Patient viewmodel with the data
        /// from the given Patient model entity.
        /// </summary>
        /// <param name="patient">Patient model entity to get values from.</param>
        public PatientViewModel(Patient patient)
        {
            Id = patient.Id;
            RegistrantName = patient.RegistrantName;
            PatientStatus = patient.PatientStatus;
            LastStatus = patient.LastStatus;
            Nhc = patient.Nhc;
            Nuhsa = patient.Nuhsa;
            Name = patient.Name;
            Surnames = patient.Surnames;
            Sex = patient.Sex;
            BirthDate = patient.BirthDate;
            BirthType = patient.BirthType;
            Ph = patient.Ph;
            Apgar = patient.Apgar;
            Weight = patient.Weight;
            CprType = patient.CprType;
            Lethargy = patient.Lethargy;
            Stupor = patient.Stupor;
            Coma = patient.Coma;
            AlteredTone = patient.AlteredTone;
            AlteredReflexes = patient.AlteredReflexes;
            AlteredSuction = patient.AlteredSuction;
            Convulsion = patient.Convulsion;
        }

        /// <summary>
        /// Creates a new Patient model entity based on the data of 
        /// this Patient viewmodel.
        /// </summary>
        public Patient ToNewModel()
        {
            var patient = new Patient
            {
                Nhc = Nhc,
                Nuhsa = Nuhsa,
                Name = Name,
                Surnames = Surnames,
                Sex = Sex,
                BirthDate = BirthDate,
                BirthType = BirthType,
                Ph = Ph,
                Apgar = Apgar,
                Weight = Weight,
                CprType = CprType,
                Lethargy = Lethargy,
                Stupor = Stupor,
                Coma = Coma,
                AlteredTone = AlteredTone,
                AlteredReflexes = AlteredReflexes,
                AlteredSuction = AlteredSuction,
                Convulsion = Convulsion,
        };

            return patient;
        }

        /// <summary>
        /// Updates the given existing Patient model entity with the values
        /// of this Patient viewmodel.
        /// </summary>
        /// <param name="patient">Patient model entity to update.</param>
        public void ToModel(Patient patient)
        {
            patient.Nhc = Nhc;
            patient.Nuhsa = Nuhsa;
            patient.Name = Name;
            patient.Surnames = Surnames;
            patient.Sex = Sex;
            patient.BirthDate = BirthDate;
            patient.BirthType = BirthType;
            patient.Ph = Ph;
            patient.Apgar = Apgar;
            patient.Weight = Weight;
            patient.CprType = CprType;
            patient.Lethargy = Lethargy;
            patient.Stupor = Stupor;
            patient.Coma = Coma;
            patient.AlteredTone = AlteredTone;
            patient.AlteredReflexes = AlteredReflexes;
            patient.AlteredSuction = AlteredSuction;
            patient.Convulsion = Convulsion;
        }
    }
}