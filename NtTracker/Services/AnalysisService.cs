using System.Data.Entity;
using System.Linq;
using NtTracker.Data;
using NtTracker.Models;
using NtTracker.ViewModels;

namespace NtTracker.Services
{
    public class AnalysisService : Service, IAnalysisService
    {
        public AnalysisService(DataContext dataContext) : base(dataContext) { }

        /// <summary>
        /// Save a new Analysis to the database with the data from a viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the new analysis data.</param>
        /// <returns>Id of the newly created Analysis.</returns>
        public int Create(AnalysisViewModel viewModel)
        {
            //Create a new Analysis from the viewmodel
            var analysis = viewModel.ToNewModel();

            Repository.Analyses.Add(analysis);
            Save();

            return analysis.Id;
        }

        /// <summary>
        /// Update the Analysis with the data from the given viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the modified data.</param>
        public void Update(AnalysisViewModel viewModel)
        {
            var analysis = Repository.Analyses.SingleOrDefault(a => a.Id == viewModel.Id);
            if (analysis == null) return;

            //Update the analysis with the viewmodel data
            viewModel.ToModel(analysis);
            Repository.Entry(analysis).State = EntityState.Modified;

            Save();
        }

        /// <summary>
        /// Delete the Analysis with the given id.
        /// </summary>
        /// <param name="id">Id of the Analysis to delete.</param>
        public void Delete(int id)
        {
            var analysis = Repository.Analyses.SingleOrDefault(a => a.Id == id);
            if (analysis == null) return;

            Repository.Analyses.Remove(analysis);
            Save();
        }
    }
}