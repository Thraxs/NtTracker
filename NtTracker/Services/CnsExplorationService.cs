using System.Data.Entity;
using System.Linq;
using NtTracker.Data;
using NtTracker.ViewModels;

namespace NtTracker.Services
{
    public class CnsExplorationService : Service, ICnsExplorationService
    {
        public CnsExplorationService(DataContext dataContext) : base(dataContext) { }

        /// <summary>
        /// Save a new CnsExploration to the database with the data from a viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the new exploration data.</param>
        /// <returns>Id of the newly created CnsExploration.</returns>
        public int Create(CnsExplorationViewModel viewModel)
        {
            //Create a new CnsExploration from the viewmodel
            var cnsExploration = viewModel.ToNewModel();

            Repository.CnsExplorations.Add(cnsExploration);
            Save();

            return cnsExploration.Id;
        }

        /// <summary>
        /// Update the CnsExploration with the data from the given viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the modified data.</param>
        public void Update(CnsExplorationViewModel viewModel)
        {
            var cnsExploration = Repository.CnsExplorations.SingleOrDefault(e => e.Id == viewModel.Id);
            if (cnsExploration == null) return;

            //Update the exploration with the viewmodel data
            viewModel.ToModel(cnsExploration);
            Repository.Entry(cnsExploration).State = EntityState.Modified;

            Save();
        }

        /// <summary>
        /// Delete the CnsExploration with the given id.
        /// </summary>
        /// <param name="id">Id of the CnsExploration to delete.</param>
        public void Delete(int id)
        {
            var cnsExploration = Repository.CnsExplorations.SingleOrDefault(e => e.Id == id);
            if (cnsExploration == null) return;

            Repository.CnsExplorations.Remove(cnsExploration);

            Save();
        }
    }
}