using System.ComponentModel.DataAnnotations;
using NtTracker.Models;
using NtTracker.Resources.NbrSurveillance;
using NtTracker.Resources.Shared;

namespace NtTracker.ViewModels
{
    public class EditNbrSurveillanceViewModel : AbstractViewModel
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

        [Display(Name = "EEG", ResourceType = typeof(SharedStrings))]
        public Electroencephalogram? Eeg { get; set; }

        [Display(Name = "AEEG", ResourceType = typeof(SharedStrings))]
        public AElectroencephalogram? AEeg { get; set; }

        [Display(Name = "TfUltrasound", ResourceType = typeof(Strings))]
        public TransfontanellarUltrasound? TfUltrasound { get; set; }

        ///
        /// Relationships
        /// 

        public CnsExplorationViewModel CnsExploration { get; set; }

        public AnalysisViewModel Analysis { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public EditNbrSurveillanceViewModel()
        {
            CnsExploration = new CnsExplorationViewModel();
            Analysis = new AnalysisViewModel();
        }

        /// <summary>
        /// Creates a new NbrSurveillance viewmodel with the data
        /// from the given NbrSurveillance model entity.
        /// </summary>
        /// <param name="nbrSurveillance">NbrSurveillance model entity to get values from.</param>
        public EditNbrSurveillanceViewModel(NbrSurveillance nbrSurveillance)
        {
            Id = nbrSurveillance.PatientId;
            TimeSlot = nbrSurveillance.TimeSlot;
            Eeg = nbrSurveillance.Eeg;
            AEeg = nbrSurveillance.AEeg;
            TfUltrasound = nbrSurveillance.TfUltrasound;
            CnsExploration = new CnsExplorationViewModel(nbrSurveillance.CnsExploration);
            Analysis = new AnalysisViewModel(nbrSurveillance.Analysis);
        }

        /// <summary>
        /// Creates a new NbrSurveillance model entity based on the data of 
        /// this NbrSurveillance viewmodel and configures the related
        /// CnsExploration with the shared attributes. 
        /// </summary>
        public NbrSurveillance ToNewModel()
        {
            var surveillance = new NbrSurveillance
            {
                TimeSlot = TimeSlot,
                Eeg = Eeg,
                AEeg = AEeg,
                TfUltrasound = TfUltrasound
            };

            return surveillance;
        }

        /// <summary>
        /// Updates the given existing NbrSurveillance model entity with the values
        /// of this NbrSurveillance viewmodel.
        /// </summary>
        /// <param name="nbrSurveillance">NbrSurveillance model entity to update.</param>
        public void ToModel(NbrSurveillance nbrSurveillance)
        {
            nbrSurveillance.Eeg = Eeg;
            nbrSurveillance.AEeg = AEeg;
            nbrSurveillance.TfUltrasound = TfUltrasound;
        }
    }
}