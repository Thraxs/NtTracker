using NtTracker.ViewModels;

namespace NtTracker.Services
{
    public interface ICnsExplorationService : IService
    {
        /// <summary>
        /// Save a new CnsExploration to the database with the data from a viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the new exploration data.</param>
        /// <returns>Id of the newly created CnsExploration.</returns>
        int Create(CnsExplorationViewModel viewModel);

        /// <summary>
        /// Update the CnsExploration with the data from the given viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the modified data.</param>
        void Update(CnsExplorationViewModel viewModel);

        /// <summary>
        /// Delete the CnsExploration with the given id.
        /// </summary>
        /// <param name="id">Id of the CnsExploration to delete.</param>
        void Delete(int id);
    }
}
