using System.ComponentModel.DataAnnotations;
using NtTracker.Models;
using NtTracker.Resources.Shared;

namespace NtTracker.ViewModels
{
    public class CnsExplorationViewModel : AbstractViewModel
    {
        ///
        /// Readonly properties
        ///

        public int Id { get; set; }

        ///
        /// Properties
        ///

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Behavior", ResourceType = typeof(SharedStrings))]
        public Behavior? Behavior { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "CranialNerves", ResourceType = typeof(SharedStrings))]
        public CranialNerves? CranialNerves { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Tone", ResourceType = typeof(SharedStrings))]
        public Tone? Tone { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Position", ResourceType = typeof(SharedStrings))]
        public Position? Position { get; set; }

        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(SharedStrings))]
        [Display(Name = "Reflexes", ResourceType = typeof(SharedStrings))]
        public Reflexes? Reflexes { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public CnsExplorationViewModel() { }

        /// <summary>
        /// Creates a new CnsExploration viewmodel with the data
        /// from the given CnsExploration model entity.
        /// </summary>
        /// <param name="cnsExploration">CnsExploration model entity to get values from.</param>
        public CnsExplorationViewModel(CnsExploration cnsExploration)
        {
            Id = cnsExploration.Id;
            Behavior = cnsExploration.Behavior;
            CranialNerves = cnsExploration.CranialNerves;
            Tone = cnsExploration.Tone;
            Position = cnsExploration.Position;
            Reflexes = cnsExploration.Reflexes;
        }

        /// <summary>
        /// Creates a new CnsExploration model entity based on the data of 
        /// this CnsExploration viewmodel.
        /// </summary>
        public CnsExploration ToNewModel()
        {
            var cnsExploration = new CnsExploration
            {
                Behavior = Behavior.GetValueOrDefault(),
                CranialNerves = CranialNerves.GetValueOrDefault(),
                Tone = Tone.GetValueOrDefault(),
                Position = Position.GetValueOrDefault(),
                Reflexes = Reflexes.GetValueOrDefault()
            };

            return cnsExploration;
        }

        /// <summary>
        /// Updates the given existing CnsExploration model entity with the values
        /// of this CnsExploration viewmodel.
        /// </summary>
        /// <param name="cnsExploration">CnsExploration model entity to update.</param>
        public void ToModel(CnsExploration cnsExploration)
        {
            cnsExploration.Behavior = Behavior.GetValueOrDefault();
            cnsExploration.CranialNerves = CranialNerves.GetValueOrDefault();
            cnsExploration.Tone = Tone.GetValueOrDefault();
            cnsExploration.Position = Position.GetValueOrDefault();
            cnsExploration.Reflexes = Reflexes.GetValueOrDefault();
        }
    }
}