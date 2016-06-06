using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NtTracker.Models
{
    public class Hypothermia
    {
        ///
        /// Properties
        ///

        [ScaffoldColumn(false)]
        [Key]
        public int Id { get; set; }

        [Required]
        [Index("IX_HypothermiaTime", 1, IsUnique = true)] //Index to avoid repeated hypothermias
        public TimeSlot TimeSlot { get; set; }

        public CnsUltrasound? CnsUs { get; set; }

        public AElectroencephalogram? AEeg { get; set; }

        public Electroencephalogram? Eeg { get; set; }

        public bool Convulsion { get; set; }

        public CerebralResonance? Cr { get; set; }

        ///
        /// Relationships
        /// 

        [Required]
        [Index("IX_HypothermiaTime", 2, IsUnique = true)]
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        [Required]
        public int AnalysisId { get; set; }
        public virtual Analysis Analysis { get; set; }

        [Required]
        public int CnsExplorationId { get; set; }
        public virtual CnsExploration CnsExploration { get; set; }
    }
}