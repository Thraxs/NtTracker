using NtTracker.Models;
using NtTracker.ViewModels;
using PagedList;

namespace NtTracker.Services
{
    public interface IPatientService : IService
    {
        /// <summary>
        /// Return the patient with the given id or null if its not found.
        /// </summary>
        /// <param name="id">Id of the patient to find.</param>
        /// <returns>Patient with the given id or null.</returns>
        Patient FindById(int id);

        /// <summary>
        /// Perform a search of patients with the given parameters and ordering.
        /// All the parameters are optional.
        /// </summary>
        /// <param name="id">Patient id.</param>
        /// <param name="nhc">Patient Clinic History Number.</param>
        /// <param name="nuhsa">Patient NUHSA.</param>
        /// <param name="name">Patient name.</param>
        /// <param name="surnames">Patient surnames.</param>
        /// <param name="status">Patient status.</param>
        /// <param name="birthFrom">Patients born after this date.</param>
        /// <param name="birthTo">Patients born before this date.</param>
        /// <param name="sorting">Sorting criteria.</param>
        /// <param name="pageNumber">Page number for listing pagination.</param>
        /// <param name="pageSize">Page size for listing pagination.</param>
        /// <returns>A paged list with the result of the search sorted as indicated.</returns>
        IPagedList<Patient> Search(string id, string nhc, string nuhsa, string name, 
            string surnames, PatientStatus? status, string birthFrom, string birthTo, 
            string sorting, int pageNumber, int pageSize);

        /// <summary>
        /// Perform a search of patients and their related entities with the given parameters and ordering.
        /// All the parameters are optional.
        /// </summary>
        /// <param name="id">Patient id.</param>
        /// <param name="nhc">Patient Clinic History Number.</param>
        /// <param name="nuhsa">Patient NUHSA.</param>
        /// <param name="name">Patient name.</param>
        /// <param name="surnames">Patient surnames.</param>
        /// <param name="status">Patient status.</param>
        /// <param name="birthFrom">Patients born after this date.</param>
        /// <param name="birthTo">Patients born before this date.</param>
        /// <param name="sorting">Sorting criteria.</param>
        /// <param name="pageNumber">Page number for listing pagination.</param>
        /// <param name="pageSize">Page size for listing pagination.</param>
        /// <returns>A paged list with the result of the search sorted as indicated.</returns>
        IPagedList<Patient> DeepSearch(string id, string nhc, string nuhsa, string name, string surnames,
            PatientStatus? status, string birthFrom, string birthTo, string sorting, int pageNumber, int pageSize);

        /// <summary>
        /// Save a new patient to the database with the data from a viewmodel. Set the 
        /// registrant of the patient with the given data.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the new patient data.</param>
        /// <param name="registrant">Username of the account that is registering the patient.</param>
        /// <param name="registrantId">Id of the account that is registering the patient.</param>
        /// <returns>Id of the newly created patient.</returns>
        int Create(PatientViewModel viewModel, string registrant, int registrantId);

        /// <summary>
        /// Update the patient with the data from the given viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the modified data.</param>
        /// <returns>bool indicating if the update was successful.</returns>
        bool Update(PatientViewModel viewModel);

        /// <summary>
        /// Delete the patient with the given id.
        /// </summary>
        /// <param name="id">Id of the patient to delete.</param>
        /// <returns>bool indicating if the deletion was successful.</returns>
        bool Delete(int id);

        /// <summary>
        /// Return the status of the patient with the given id, 
        /// or null if the patient doesn't exist.
        /// </summary>
        /// <param name="id">Id of the patient.</param>
        /// <returns>Status of the patient with the given id.</returns>
        PatientStatus GetStatus(int id);

        /// <summary>
        /// Check for changes in the patient data to update his status.
        /// </summary>
        /// <param name="patientId">Id of the patient to check status of.</param>
        void UpdateStatus(int patientId);

        /// <summary>
        /// Checks if the tracking for the patient with the 
        /// given id is closed.
        /// </summary>
        /// <param name="id">Id of the patient.</param>
        /// <returns>bool indicating if the tracking of the given
        /// patient is closed.</returns>
        bool IsClosed(int id);

        /// <summary>
        /// Closes the tracking for the patient with the
        /// given id.
        /// </summary>
        /// <param name="id">Id of the patient.</param>
        void ClosePatientTracking(int id);

        /// <summary>
        /// Opens the tracking for the patient with the
        /// given id.
        /// </summary>
        /// <param name="id">Id of the patient.</param>
        void OpenPatientTracking(int id);
    }
}
