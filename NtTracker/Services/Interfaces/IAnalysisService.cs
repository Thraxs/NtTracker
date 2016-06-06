using NtTracker.ViewModels;

namespace NtTracker.Services
{
    public interface IAnalysisService : IService
    {
        /// <summary>
        /// Save a new Analysis to the database with the data from a viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the new analysis data.</param>
        /// <returns>Id of the newly created Analysis.</returns>
        int Create(AnalysisViewModel viewModel);

        /// <summary>
        /// Update the Analysis with the data from the given viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the modified data.</param>
        void Update(AnalysisViewModel viewModel);

        /// <summary>
        /// Delete the Analysis with the given id.
        /// </summary>
        /// <param name="id">Id of the Analysis to delete.</param>
        void Delete(int id);
    }
}