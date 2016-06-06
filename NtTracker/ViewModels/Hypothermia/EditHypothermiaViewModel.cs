using System.ComponentModel.DataAnnotations;
using NtTracker.Models;
using NtTracker.Resources.Hypothermia;
using NtTracker.Resources.Shared;

namespace NtTracker.ViewModels
{
    public class EditHypothermiaViewModel : AbstractViewModel
    {
        ///
        /// Readonly properties
        ///

        public int Id { get; set; }

        ///
        /// Properties
        ///

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "TimeSlot", ResourceType = typeof(SharedStrings))]
        public TimeSlot TimeSlot { get; set; }

        [Display(Name = "CnsUltrasound", ResourceType = typeof(Strings))]
        public CnsUltrasound? CnsUs { get; set; }

        [Display(Name = "AEEG", ResourceType = typeof(SharedStrings))]
        public AElectroencephalogram? AEeg { get; set; }

        [Display(Name = "EEG", ResourceType = typeof(SharedStrings))]
        public Electroencephalogram? Eeg { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Convulsion", ResourceType = typeof(SharedStrings))]
        public bool Convulsion { get; set; }

        [Display(Name = "CerebralResonance", ResourceType = typeof(Strings))]
        public CerebralResonance? Cr { get; set; }

        ///
        /// Relationships
        /// 

        public CnsExplorationViewModel CnsExploration { get; set; }

        public AnalysisViewModel Analysis { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public EditHypothermiaViewModel()
        {
            CnsExploration = new CnsExplorationViewModel();
            Analysis = new AnalysisViewModel();
        }

        /// <summary>
        /// Creates a new Hypothermia viewmodel with the data
        /// from the given Hypothermia model entity.
        /// </summary>
        /// <param name="hypothermia">Hypothermia model entity to get values from.</param>
        public EditHypothermiaViewModel(Hypothermia hypothermia)
        {
            TimeSlot = hypothermia.TimeSlot;
            CnsUs = hypothermia.CnsUs;
            AEeg = hypothermia.AEeg;
            Eeg = hypothermia.Eeg;
            Convulsion = hypothermia.Convulsion;
            Cr = hypothermia.Cr;
            CnsExploration = new CnsExplorationViewModel(hypothermia.CnsExploration);
            Analysis = new AnalysisViewModel(hypothermia.Analysis);
        }

        /// <summary>
        /// Creates a new Hypothermia model entity based on the data of 
        /// this Hypothermia viewmodel and configures the related
        /// CnsExploration with the shared attributes. 
        /// </summary>
        public Hypothermia ToNewModel()
        {
            var hypothermia = new Hypothermia
            {
                TimeSlot = TimeSlot,
                CnsUs = CnsUs,
                AEeg = AEeg,
                Eeg = Eeg,
                Convulsion = Convulsion,
                Cr = Cr
            };

            return hypothermia;
        }

        /// <summary>
        /// Updates the given existing Hypothermia model entity with the values
        /// of this Hypothermia viewmodel.
        /// </summary>
        /// <param name="hypothermia">Hypothermia model entity to update.</param>
        public void ToModel(Hypothermia hypothermia)
        {
            hypothermia.CnsUs = CnsUs;
            hypothermia.AEeg = AEeg;
            hypothermia.Eeg = Eeg;
            hypothermia.Convulsion = Convulsion;
            hypothermia.Cr = Cr;
        }
    }
}