using System.ComponentModel.DataAnnotations;
using NtTracker.Models;
using NtTracker.Resources.Analysis;

namespace NtTracker.ViewModels
{
    public class AnalysisViewModel
    {
        ///
        /// Readonly properties
        ///

        public int Id { get; set; }

        ///
        /// Properties
        ///

        [Display(Name = "Hemoglobin", ResourceType = typeof(Strings))]
        public double? Hemoglobin { get; set; }

        [Display(Name = "Hematocrit", ResourceType = typeof(Strings))]
        public double? Hematocrit { get; set; }

        [Display(Name = "PlateletCount", ResourceType = typeof(Strings))]
        public double? PlateletCount { get; set; }

        [Display(Name = "ALT", ResourceType = typeof(Strings))]
        public double? Alt { get; set; }

        [Display(Name = "AST", ResourceType = typeof(Strings))]
        public double? Ast { get; set; }

        [Display(Name = "CPK", ResourceType = typeof(Strings))]
        public double? Cpk { get; set; }

        [Display(Name = "Proteins", ResourceType = typeof(Strings))]
        public double? Proteins { get; set; }

        [Display(Name = "Sodium", ResourceType = typeof(Strings))]
        public double? Sodium { get; set; }

        [Display(Name = "Potassium", ResourceType = typeof(Strings))]
        public double? Potassium { get; set; }

        [Display(Name = "Chloride", ResourceType = typeof(Strings))]
        public double? Chloride { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public AnalysisViewModel() { }

        /// <summary>
        /// Creates a new Analysis viewmodel with the data
        /// from the given Analysis model entity.
        /// </summary>
        /// <param name="analysis">Analysis model entity to get values from.</param>
        public AnalysisViewModel(Analysis analysis)
        {
            Id = analysis.Id;
            Hemoglobin = analysis.Hemoglobin;
            Hematocrit = analysis.Hematocrit;
            PlateletCount = analysis.PlateletCount;
            Alt = analysis.Alt;
            Ast = analysis.Ast;
            Cpk = analysis.Cpk;
            Proteins = analysis.Proteins;
            Sodium = analysis.Sodium;
            Potassium = analysis.Potassium;
            Chloride = analysis.Chloride;
        }

        /// <summary>
        /// Creates a new Analysis model entity based on the data of 
        /// this Analysis viewmodel.
        /// </summary>
        public Analysis ToNewModel()
        {
            var analysis = new Analysis
            {
                Hemoglobin = Hemoglobin,
                Hematocrit = Hematocrit,
                PlateletCount = PlateletCount,
                Alt = Alt,
                Ast = Ast,
                Cpk = Cpk,
                Proteins = Proteins,
                Sodium = Sodium,
                Potassium = Potassium,
                Chloride = Chloride
            };

            return analysis;
        }

        /// <summary>
        /// Updates the given existing Analysis model entity with the values
        /// of this Analysis viewmodel.
        /// </summary>
        /// <param name="analysis">Analysis model entity to update.</param>
        public void ToModel(Analysis analysis)
        {
            analysis.Hemoglobin = Hemoglobin;
            analysis.Hematocrit = Hematocrit;
            analysis.PlateletCount = PlateletCount;
            analysis.Alt = Alt;
            analysis.Ast = Ast;
            analysis.Cpk = Cpk;
            analysis.Proteins = Proteins;
            analysis.Sodium = Sodium;
            analysis.Potassium = Potassium;
            analysis.Chloride = Chloride;
        }
    }
}