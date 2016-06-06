using System.Collections.Generic;
using NtTracker.Models;
using NtTracker.ViewModels;

namespace NtTracker.Services
{
    public interface IMonitoringService : IService
    {
        /// <summary>
        /// Find the Monitoring with the given id.
        /// </summary>
        /// <param name="id">Monitoring id.</param>
        /// <returns>The Monitoring with the given id or null if not found.</returns>
        Monitoring FindById(int id);
        
        /// <summary>
        /// Returns the monitorings for the given patient, ordered by time.
        /// </summary>
        /// <param name="patientId">ID of the patient.</param>
        /// <returns>List of monitorings for that patient ordered by time.</returns>
        List<Monitoring> FindByPatientId(int patientId);

        /// <summary>
        /// Save a new Monitoring to the database with the data from a viewmodel. If the TimeSlot 
        /// is repeated, an error will be added to the modelstate of the viewmodel and the operation
        /// will not be completed.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the new monitoring data.</param>
        /// <param name="patientId">Id of the patient for which the monitoring is being created.</param>
        void Create(MonitoringViewModel viewModel, int patientId);

        /// <summary>
        /// Update the Monitoring with the data from the given viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the modified data.</param>
        /// <returns>bool indicating if the update was successful.</returns>
        bool Update(MonitoringViewModel viewModel);

        /// <summary>
        /// Delete the Monitoring with the given id.
        /// </summary>
        /// <param name="id">Id of the Monitoring to delete.</param>
        /// <returns>bool indicating if the deletion was successful.</returns>
        bool Delete(int id);
    }
}
