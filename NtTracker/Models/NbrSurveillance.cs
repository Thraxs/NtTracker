using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NtTracker.Models
{
    public class NbrSurveillance
    {
        ///
        /// Properties
        ///

        [ScaffoldColumn(false)]
        [Key]
        public int Id { get; set; }

        [Required]
        [Index("IX_NbrSurveillanceTime", 1, IsUnique = true)] //Index to avoid repeated surveillances
        public TimeSlot TimeSlot { get; set; }

        public Electroencephalogram? Eeg { get; set; }

        public AElectroencephalogram? AEeg { get; set; }

        public TransfontanellarUltrasound? TfUltrasound { get; set; }

        ///
        /// Relationships
        /// 

        [Required]
        [Index("IX_NbrSurveillanceTime", 2, IsUnique = true)]
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