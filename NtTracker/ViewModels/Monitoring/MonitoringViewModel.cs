using System;
using System.ComponentModel.DataAnnotations;
using NtTracker.Models;
using NtTracker.Resources.Monitoring;
using NtTracker.Resources.Shared;

namespace NtTracker.ViewModels
{
    public class MonitoringViewModel : AbstractViewModel
    {
        ///
        /// Readonly properties
        ///

        public int Id { get; set; }

        ///
        /// Properties
        ///

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "DateTime", ResourceType = typeof(SharedStrings))]
        public DateTime DateTime { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [StringLength(20, MinimumLength = 1, ErrorMessageResourceName = "Validation_StringLength", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Description", ResourceType = typeof(Strings))]
        public string Description { get; set; }

        [StringLength(300, ErrorMessageResourceName = "Validation_StringLength", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Comments", ResourceType = typeof(Strings))]
        public string Comments { get; set; }


        // --- Motor data ---

        [Display(Name = "MuscularTone", ResourceType = typeof(Strings))]
        public MuscularTone? MuscularTone { get; set; }

        [Display(Name = "Spasticity", ResourceType = typeof(Strings))]
        public bool Spasticity { get; set; }

        [Display(Name = "Dystonia", ResourceType = typeof(Strings))]
        public bool Dystonia { get; set; }

        [Display(Name = "Dyskinesia", ResourceType = typeof(Strings))]
        public bool Dyskinesia { get; set; }

        [Display(Name = "Ataxia", ResourceType = typeof(Strings))]
        public bool Ataxia { get; set; }

        [Display(Name = "Hyperreflexia", ResourceType = typeof(Strings))]
        public bool Hyperreflexia { get; set; }

        [Display(Name = "DevelopmentReflexes", ResourceType = typeof(Strings))]
        public DevelopmentReflexes? DevelopmentReflexes { get; set; }

        // --- Cognitive data ---

        [Display(Name = "CognitiveDeficit", ResourceType = typeof(Strings))]
        public CognitiveDeficit? CognitiveDeficit { get; set; }

        [Display(Name = "PrimaryReflexes", ResourceType = typeof(Strings))]
        public bool PrimaryReflexes { get; set; }

        [Display(Name = "AuditoryDeficit", ResourceType = typeof(Strings))]
        public AuditoryDeficit? AuditoryDeficit { get; set; }

        [Display(Name = "VisualDeficit", ResourceType = typeof(Strings))]
        public VisualDeficit? VisualDeficit { get; set; }

        [Display(Name = "VocalDeficit", ResourceType = typeof(Strings))]
        public VocalDeficit? VocalDeficit { get; set; }


        // --- Emotional data ---

        [Display(Name = "TactileResponse", ResourceType = typeof(Strings))]
        public EmotionalResponse? TactileResponse { get; set; }

        [Display(Name = "SoundResponse", ResourceType = typeof(Strings))]
        public EmotionalResponse? SoundResponse { get; set; }

        [Display(Name = "CaretakerResponse", ResourceType = typeof(Strings))]
        public EmotionalResponse? CaretakerResponse { get; set; }

        [Display(Name = "StrangersResponse", ResourceType = typeof(Strings))]
        public EmotionalResponse? StrangersResponse { get; set; }

        [Display(Name = "Smiles", ResourceType = typeof(Strings))]
        public bool Smiles { get; set; }

        ///
        /// Relationships
        /// 

        public int PatientId { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public MonitoringViewModel() { }

        /// <summary>
        /// Creates a new Monitoring viewmodel with the data
        /// from the given Monitoring model entity.
        /// </summary>
        /// <param name="monitoring">Monitoring model entity to get values from.</param>
        public MonitoringViewModel(Monitoring monitoring)
        {
            DateTime = monitoring.DateTime;
            Description = monitoring.Description;
            Comments = monitoring.Comments;
            MuscularTone = monitoring.MuscularTone;
            Spasticity = monitoring.Spasticity;
            Dystonia = monitoring.Dystonia;
            Dyskinesia = monitoring.Dyskinesia;
            Ataxia = monitoring.Ataxia;
            Hyperreflexia = monitoring.Hyperreflexia;
            DevelopmentReflexes = monitoring.DevelopmentReflexes;
            CognitiveDeficit = monitoring.CognitiveDeficit;
            PrimaryReflexes = monitoring.PrimaryReflexes;
            AuditoryDeficit = monitoring.AuditoryDeficit;
            VisualDeficit = monitoring.VisualDeficit;
            VocalDeficit = monitoring.VocalDeficit;
            TactileResponse = monitoring.TactileResponse;
            SoundResponse = monitoring.SoundResponse;
            CaretakerResponse = monitoring.CaretakerResponse;
            StrangersResponse = monitoring.StrangersResponse;
            Smiles = monitoring.Smiles;
            PatientId = monitoring.PatientId;
        }

        /// <summary>
        /// Creates a new Monitoring model entity based on the data of 
        /// this Monitoring viewmodel.
        /// </summary>
        public Monitoring ToNewModel()
        {
            var monitoring = new Monitoring
            {
                DateTime = DateTime,
                Description = Description,
                Comments = Comments,
                MuscularTone = MuscularTone,
                Spasticity = Spasticity,
                Dystonia = Dystonia,
                Dyskinesia = Dyskinesia,
                Ataxia = Ataxia,
                Hyperreflexia = Hyperreflexia,
                DevelopmentReflexes = DevelopmentReflexes,
                CognitiveDeficit = CognitiveDeficit,
                PrimaryReflexes = PrimaryReflexes,
                AuditoryDeficit = AuditoryDeficit,
                VisualDeficit = VisualDeficit,
                VocalDeficit = VocalDeficit,
                TactileResponse = TactileResponse,
                SoundResponse = SoundResponse,
                CaretakerResponse = CaretakerResponse,
                StrangersResponse = StrangersResponse,
                Smiles = Smiles
            };

            return monitoring;
        }

        /// <summary>
        /// Updates the given existing Monitoring model entity with the values
        /// of this Monitoring viewmodel.
        /// </summary>
        /// <param name="monitoring">Monitoring model entity to update.</param>
        public void ToModel(Monitoring monitoring)
        {
            monitoring.DateTime = DateTime;
            monitoring.Description = Description;
            monitoring.Comments = Comments;
            monitoring.MuscularTone = MuscularTone;
            monitoring.Spasticity = Spasticity;
            monitoring.Dystonia = Dystonia;
            monitoring.Dyskinesia = Dyskinesia;
            monitoring.Ataxia = Ataxia;
            monitoring.Hyperreflexia = Hyperreflexia;
            monitoring.DevelopmentReflexes = DevelopmentReflexes;
            monitoring.CognitiveDeficit = CognitiveDeficit;
            monitoring.PrimaryReflexes = PrimaryReflexes;
            monitoring.AuditoryDeficit = AuditoryDeficit;
            monitoring.VisualDeficit = VisualDeficit;
            monitoring.VocalDeficit = VocalDeficit;
            monitoring.TactileResponse = TactileResponse;
            monitoring.SoundResponse = SoundResponse;
            monitoring.CaretakerResponse = CaretakerResponse;
            monitoring.StrangersResponse = StrangersResponse;
            monitoring.Smiles = Smiles;
        }
    }
}