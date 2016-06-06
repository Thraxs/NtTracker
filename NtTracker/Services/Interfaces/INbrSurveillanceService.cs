using System.Collections.Generic;
using NtTracker.Models;
using NtTracker.ViewModels;

namespace NtTracker.Services
{
    public interface INbrSurveillanceService : IService
    {
        /// <summary>
        /// Find the NbrSurveillance with the given id. Include also
        /// the related analysis and exploration.
        /// </summary>
        /// <param name="id">NbrSurveillance id.</param>
        /// <returns>The NbrSurveillance with the given id 
        /// or null if not found.</returns>
        NbrSurveillance FindById(int id);

        /// <summary>
        /// Find the NbrSurveillances of the patient with the given id.
        /// </summary>
        /// <param name="patientId">Patient id.</param>
        /// <returns>NbrSurveillances for the patient with the given id.</returns>
        List<NbrSurveillance> FindByPatientId(int patientId);

        /// <summary>
        /// Checks with the data of the surveillance if there is risk
        /// of encephalopathy.
        /// </summary>
        /// <param name="nbrSurveillance">NbrSurveillance to check.</param>
        /// <returns>bool indicating if there is risk of encephalopathy.</returns>
        bool AtRisk(NbrSurveillance nbrSurveillance);

        /// <summary>
        /// Save a new NbrSurveillance and it's related CnsExploration to the database with the data from 
        /// a viewmodel. If the TimeSlot is repeated, an error will be added to the modelstate of the 
        /// viewmodel and the operation will not be completed.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the new surveillance data.</param>
        /// <param name="patientId">Id of the patient for which the surveillance is being created.</param>
        void Create(EditNbrSurveillanceViewModel viewModel, int patientId);

        /// <summary>
        /// Update the NbrSurveillance with the data from the given viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the modified data.</param>
        /// <returns>bool indicating if the update was successful.</returns>
        bool Update(EditNbrSurveillanceViewModel viewModel);

        /// <summary>
        /// Delete the NbrSurveillance with the given id.
        /// </summary>
        /// <param name="id">Id of the NbrSurveillance to delete.</param>
        /// <returns>bool indicating if the deletion was successful.</returns>
        bool Delete(int id);
    }
}
