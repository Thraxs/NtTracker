using System;
using System.ComponentModel.DataAnnotations;

namespace NtTracker.Models
{
    public class Monitoring
    {
        ///
        /// Properties
        ///

        [ScaffoldColumn(false)]
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [StringLength(20)]
        public string Description { get; set; }

        [StringLength(300)]
        public string Comments { get; set; }

        // --- Motor data ---

        public MuscularTone? MuscularTone { get; set; }

        public bool Spasticity { get; set; }

        public bool Dystonia { get; set; }

        public bool Dyskinesia { get; set; }

        public bool Ataxia { get; set; }

        public bool Hyperreflexia { get; set; }

        public DevelopmentReflexes? DevelopmentReflexes { get; set; }

        // --- Cognitive data ---

        public CognitiveDeficit? CognitiveDeficit { get; set; }

        public bool PrimaryReflexes { get; set; }

        public AuditoryDeficit? AuditoryDeficit { get; set; }

        public VisualDeficit? VisualDeficit { get; set; }

        public VocalDeficit? VocalDeficit { get; set; }


        // --- Emotional data ---

        public EmotionalResponse? TactileResponse { get; set; }

        public EmotionalResponse? SoundResponse { get; set; }

        public EmotionalResponse? CaretakerResponse { get; set; }

        public EmotionalResponse? StrangersResponse { get; set; }

        public bool Smiles { get; set; }

        ///
        /// Relationships
        /// 

        [Required]
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }
    }
}