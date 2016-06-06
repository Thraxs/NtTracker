using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using NtTracker.Data;
using NtTracker.Models;
using NtTracker.ViewModels;

namespace NtTracker.Services
{
    public class MonitoringService : Service, IMonitoringService
    {
        public MonitoringService(DataContext dataContext) : base(dataContext) { }

        /// <summary>
        /// Find the Monitoring with the given id.
        /// </summary>
        /// <param name="id">Monitoring id.</param>
        /// <returns>The Monitoring with the given id or null if not found.</returns>
        public Monitoring FindById(int id)
        {
            var monitoring = Repository.Monitorings
                .SingleOrDefault(e => e.Id == id);

            return monitoring;
        }

        /// <summary>
        /// Returns the monitorings for the given patient, ordered by time.
        /// </summary>
        /// <param name="patientId">ID of the patient.</param>
        /// <returns>List of monitorings for that patient ordered by time.</returns>
        public List<Monitoring> FindByPatientId(int patientId)
        {
            var monitorings = Repository.Monitorings
                .Where(m => m.PatientId == patientId)
                .OrderByDescending(m => m.DateTime);

            return monitorings.ToList();
        }

        /// <summary>
        /// Save a new Monitoring to the database with the data from a viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the new monitoring data.</param>
        /// <param name="patientId">Id of the patient for which the monitoring is being created.</param>
        public void Create(MonitoringViewModel viewModel, int patientId)
        {
            //Create a new Monitoring from the viewmodel
            var monitoring = viewModel.ToNewModel();
            monitoring.PatientId = patientId;

            Repository.Monitorings.Add(monitoring);
            Save();
        }

        /// <summary>
        /// Update the Monitoring with the data from the given viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the modified data.</param>
        /// <returns>bool indicating if the update was successful.</returns>
        public bool Update(MonitoringViewModel viewModel)
        {
            var monitoring = Repository.Monitorings.SingleOrDefault(p => p.Id == viewModel.Id);
            if (monitoring == null) return false;

            //Update the monitoring with the viewmodel data
            viewModel.ToModel(monitoring);
            Repository.Entry(monitoring).State = EntityState.Modified;

            Save();
            return true;
        }

        /// <summary>
        /// Delete the Monitoring with the given id.
        /// </summary>
        /// <param name="id">Id of the Monitoring to delete.</param>
        /// <returns>bool indicating if the deletion was successful.</returns>
        public bool Delete(int id)
        {
            var monitoring = Repository.Monitorings.SingleOrDefault(e => e.Id == id);
            if (monitoring == null) return false;

            Repository.Monitorings.Remove(monitoring);

            Save();
            return true;
        }
    }
}