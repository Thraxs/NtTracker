using System.ComponentModel.DataAnnotations;

namespace NtTracker.Models
{
    public class CnsExploration
    {
        ///
        /// Properties
        ///

        [ScaffoldColumn(false)]
        [Key]
        public int Id { get; set; }

        [Required]
        public Behavior Behavior { get; set; }

        [Required]
        public CranialNerves CranialNerves { get; set; }

        [Required]
        public Tone Tone { get; set; }

        [Required]
        public Position Position { get; set; }

        [Required]
        public Reflexes Reflexes { get; set; }

        ///
        /// Derived properties
        ///

        /// <summary>
        /// Returns the computed score (out of a max of 10) of this exploration. 
        /// </summary>
        public int Score => (int) Behavior + (int) CranialNerves + (int) Tone + (int) Position + (int) Reflexes;
    }
}