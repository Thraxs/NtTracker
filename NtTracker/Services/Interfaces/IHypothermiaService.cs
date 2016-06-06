using System.Collections.Generic;
using NtTracker.Models;
using NtTracker.ViewModels;

namespace NtTracker.Services
{
    public interface IHypothermiaService : IService
    {
        /// <summary>
        /// Find the Hypothermia with the given id. Include also
        /// the related analysis.
        /// </summary>
        /// <param name="id">Hypothermia id.</param>
        /// <returns>The Hypothermia with the given id 
        /// or null if not found.</returns>
        Hypothermia FindById(int id);

        /// <summary>
        /// Find the hypothermias of the patient with the given id.
        /// </summary>
        /// <param name="patientId">Id of the patient.</param>
        /// <returns>List of hypothermias for the patient with the given id.</returns>
        List<Hypothermia> FindByPatientId(int patientId);

        /// <summary>
        /// Save a new Hypothermia and it's related CnsExploration to the database with the data from 
        /// a viewmodel. If the TimeSlot is repeated, an error will be added to the modelstate of the 
        /// viewmodel and the operation will not be completed.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the new hypothermia data.</param>
        /// <param name="patientId">Id of the patient for which the hypothermia is being created.</param>
        void Create(EditHypothermiaViewModel viewModel, int patientId);

        /// <summary>
        /// Update the Hypothermia with the data from the given viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the modified data.</param>
        /// <returns>bool indicating if the update was successful.</returns>
        bool Update(EditHypothermiaViewModel viewModel);

        /// <summary>
        /// Delete the Hypothermia with the given id.
        /// </summary>
        /// <param name="id">Id of the Hypothermia to delete.</param>
        /// <returns>bool indicating if the deletion was successful.</returns>
        bool Delete(int id);
    }
}
