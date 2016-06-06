using System;
using System.Data.Entity;
using System.Linq;
using NtTracker.Data;
using NtTracker.Models;
using NtTracker.ViewModels;
using PagedList;

namespace NtTracker.Services
{
    public class PatientService : Service, IPatientService
    {
        private readonly INbrSurveillanceService _nbrSurveillanceService;
        private readonly IHypothermiaService _hypothermiaService;

        public PatientService(DataContext dataContext, INbrSurveillanceService nbrSurveillanceService, 
            IHypothermiaService hypothermiaService) : base(dataContext)
        {
            _nbrSurveillanceService = nbrSurveillanceService;
            _hypothermiaService = hypothermiaService;
        }

        /// <summary>
        /// Return the patient with the given id or null if its not found.
        /// </summary>
        /// <param name="id">Id of the patient to find.</param>
        /// <returns>Patient with the given id or null.</returns>
        public Patient FindById(int id)
        {
            var patient = Repository.Patients
                .SingleOrDefault(p => p.Id == id);

            return patient;
        }

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
        public IPagedList<Patient> Search(string id, string nhc, string nuhsa, string name, string surnames, 
            PatientStatus? status, string birthFrom, string birthTo, string sorting, int pageNumber, int pageSize)
        {
            var patients = Repository.Patients.AsQueryable();
            return FilterAndPage(patients, id, nhc, nuhsa, name, surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);
        }

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
        public IPagedList<Patient> DeepSearch(string id, string nhc, string nuhsa, string name, string surnames,
            PatientStatus? status, string birthFrom, string birthTo, string sorting, int pageNumber, int pageSize)
        {
            var patients = Repository.Patients.AsQueryable()
                .Include(p => p.NbrSurveillances.Select(s => s.CnsExploration))
                .Include(p => p.NbrSurveillances.Select(s => s.Analysis))
                .Include(p => p.Hypothermias)
                .Include(p => p.Hypothermias.Select(h => h.CnsExploration))
                .Include(p => p.Hypothermias.Select(h => h.Analysis))
                .Include(p => p.Monitorings);

            return FilterAndPage(patients, id, nhc, nuhsa, name, surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);
        }

        /// <summary>
        /// Apply the given filter, sort and paging to the given patients query.
        /// </summary>
        /// <param name="patients">Initial patients query object.</param>
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
        /// <returns>PagedList of patients with the given criteria applied.</returns>
        private static IPagedList<Patient> FilterAndPage(IQueryable<Patient> patients, string id, string nhc, string nuhsa, string name, 
            string surnames, PatientStatus? status, string birthFrom, string birthTo, string sorting, int pageNumber, int pageSize)
        {
            //Filtering
            if (!string.IsNullOrEmpty(id))
            {
                int idComp;
                if (!int.TryParse(id, out idComp))
                {
                    idComp = -1;
                }
                patients = patients.Where(p => p.Id == idComp);
            }
            if (!string.IsNullOrEmpty(nhc))
            {
                patients = patients.Where(p => p.Nhc.Contains(nhc));
            }
            if (!string.IsNullOrEmpty(nuhsa))
            {
                patients = patients.Where(p => p.Nuhsa.Contains(nuhsa));
            }
            if (!string.IsNullOrEmpty(name))
            {
                patients = patients.Where(p => p.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(surnames))
            {
                patients = patients.Where(p => p.Surnames.Contains(surnames));
            }
            if (status != null)
            {
                patients = patients.Where(p => p.PatientStatus == status);
            }
            if (!string.IsNullOrEmpty(birthFrom))
            {
                DateTime birthComp;
                if (!DateTime.TryParse(birthFrom, out birthComp))
                {
                    birthComp = DateTime.MaxValue;
                }
                patients = patients.Where(p => p.BirthDate >= birthComp);
            }
            if (!string.IsNullOrEmpty(birthTo))
            {
                DateTime birthComp;
                if (!DateTime.TryParse(birthTo, out birthComp))
                {
                    birthComp = DateTime.MinValue;
                }
                patients = patients.Where(p => p.BirthDate <= birthComp);
            }

            //Sorting
            switch (sorting)
            {
                case "id_a":
                    patients = patients.OrderBy(p => p.Id);
                    break;
                case "id_d":
                    patients = patients.OrderByDescending(p => p.Id);
                    break;
                case "nhc_a":
                    patients = patients.OrderBy(p => p.Nhc);
                    break;
                case "nhc_d":
                    patients = patients.OrderByDescending(p => p.Nhc);
                    break;
                case "nuhsa_a":
                    patients = patients.OrderBy(p => p.Nuhsa);
                    break;
                case "nuhsa_d":
                    patients = patients.OrderByDescending(p => p.Nuhsa);
                    break;
                case "name_a":
                    patients = patients.OrderBy(p => p.Name);
                    break;
                case "name_d":
                    patients = patients.OrderByDescending(p => p.Name);
                    break;
                case "surnames_a":
                    patients = patients.OrderBy(p => p.Surnames);
                    break;
                case "surnames_d":
                    patients = patients.OrderByDescending(p => p.Surnames);
                    break;
                case "birthdate_a":
                    patients = patients.OrderBy(p => p.BirthDate);
                    break;
                case "birthdate_d":
                    patients = patients.OrderByDescending(p => p.BirthDate);
                    break;
                case "status_a":
                    patients = patients.OrderBy(p => p.PatientStatus);
                    break;
                case "status_d":
                    patients = patients.OrderByDescending(p => p.PatientStatus);
                    break;
                default:
                    patients = patients.OrderByDescending(p => p.Id);
                    break;
            }

            return patients.ToPagedList(pageNumber, pageSize);
        }

        /// <summary>
        /// Save a new patient to the database with the data from a viewmodel. Set the 
        /// registrant of the patient with the given data.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the new patient data.</param>
        /// <param name="registrant">Username of the account that is registering the patient.</param>
        /// <param name="registrantId">Id of the account that is registering the patient.</param>
        /// <returns>Id of the newly created patient.</returns>
        public int Create(PatientViewModel viewModel, string registrant, int registrantId)
        {
            //Create a new patient from the viewmodel
            var patient = viewModel.ToNewModel();

            //Set initial status to normal
            patient.PatientStatus = PatientStatus.Normal;

            //Set the registrant fields with the logged user data
            patient.RegistrantName = registrant;
            patient.RegistrantId = registrantId;

            Repository.Patients.Add(patient);
            Save();

            //Check if status needs to be updated
            UpdateStatus(patient.Id);

            return patient.Id;
        }

        /// <summary>
        /// Update the patient with the data from the given viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the modified data.</param>
        /// <returns>bool indicating if the update was successful.</returns>
        public bool Update(PatientViewModel viewModel)
        {
            var patient = Repository.Patients.SingleOrDefault(p => p.Id == viewModel.Id);
            if (patient == null) return false;

            //Update the patient with the viewmodel data
            viewModel.ToModel(patient);
            Repository.Entry(patient).State = EntityState.Modified;

            Save();

            //Check if status needs to be updated
            UpdateStatus(viewModel.Id);

            return true;
        }

        /// <summary>
        /// Delete the patient with the given id.
        /// </summary>
        /// <param name="id">Id of the patient to delete.</param>
        /// <returns>bool indicating if the deletion was successful.</returns>
        public bool Delete(int id)
        {
            var exists = Repository.Patients.Any(p => p.Id == id);
            if (!exists) return false;

            //Delete related NbrSurveillances
            var surveillances = _nbrSurveillanceService.FindByPatientId(id);
            surveillances.ForEach(s => _nbrSurveillanceService.Delete(s.Id));

            //Delete related Hypothermias
            var hypothermias = _hypothermiaService.FindByPatientId(id);
            hypothermias.ForEach(h => _hypothermiaService.Delete(h.Id));

            //Load related entities and delete patient
            var patient = new Patient {Id = id};
            Repository.Patients.Attach(patient);

            Repository.Entry(patient).Collection(p => p.Operations).Load();
            Repository.Patients.Remove(patient);

            Save();
            return true;
        }

        /// <summary>
        /// Return the status of the patient with the given id, 
        /// or null if the patient doesn't exist.
        /// </summary>
        /// <param name="id">Id of the patient.</param>
        /// <returns>Status of the patient with the given id.</returns>
        public PatientStatus GetStatus(int id)
        {
            var status = Repository.Patients
                .Where(p => p.Id == id)
                .Select(p => p.PatientStatus)
                .SingleOrDefault();

            return status;
        }

        /// <summary>
        /// Check for changes in the patient data to update his status.
        /// </summary>
        /// <param name="patientId">Id of the patient to check status of.</param>
        public void UpdateStatus(int patientId)
        {
            var patient = Repository.Patients
                .Include(p => p.NbrSurveillances)
                .Single(p => p.Id == patientId);

            var patientStatus = PatientStatus.Normal;
            //If status is normal, check for NbrSurveillance and Hypothermia
            if (patient.PatientStatus == PatientStatus.Normal)
            {
                //Patient fulfills first criteria
                if (patient.Apgar <= 5 || patient.CprType == CprType.Type1 || patient.Ph < 7)
                {
                    //Patient fulfills second criteria
                    if ((patient.Lethargy || patient.Stupor || patient.Coma) &&
                        (patient.AlteredTone || patient.AlteredReflexes || patient.AlteredSuction || patient.Convulsion))
                    {
                        //Set to Hypothermia
                        patientStatus = PatientStatus.Hypothermia;
                    }
                    else
                    {
                        //Set to NbrSurveillance
                        patientStatus = PatientStatus.NbrSurveillance;
                    }
                }
                
            }
            //If status is NbrSurveillance, check for monitoring
            else if (patient.PatientStatus == PatientStatus.NbrSurveillance)
            {
                var surveillances = _nbrSurveillanceService.FindByPatientId(patientId);
                foreach (var surveillance in surveillances)
                {
                    //Check surveillance score
                    if (_nbrSurveillanceService.AtRisk(surveillance))
                    {
                        //If at risk, set status to Monitoring
                        patientStatus = PatientStatus.Monitoring;
                    }
                }
            }

            //Status didn't change, exit method
            if (patientStatus == PatientStatus.Normal) return;

            patient.PatientStatus = patientStatus;
            Repository.Entry(patient).State = EntityState.Modified;
            Save();
        }

        /// <summary>
        /// Checks if the tracking for the patient with the 
        /// given id is closed.
        /// </summary>
        /// <param name="id">Id of the patient.</param>
        /// <returns>bool indicating if the tracking of the given
        /// patient is closed.</returns>
        public bool IsClosed(int id)
        {
            var status = GetStatus(id);
            return status.Equals(PatientStatus.Closed);
        }

        /// <summary>
        /// Closes the tracking for the patient with the
        /// given id.
        /// </summary>
        /// <param name="id">Id of the patient.</param>
        public void ClosePatientTracking(int id)
        {
            var patient = Repository.Patients.SingleOrDefault(p => p.Id == id);
            if (patient == null) return;

            patient.LastStatus = patient.PatientStatus;
            patient.PatientStatus = PatientStatus.Closed;

            Repository.Entry(patient).State = EntityState.Modified;
            Save();
        }

        /// <summary>
        /// Opens the tracking for the patient with the
        /// given id.
        /// </summary>
        /// <param name="id">Id of the patient.</param>
        public void OpenPatientTracking(int id)
        {
            var patient = Repository.Patients.SingleOrDefault(p => p.Id == id);
            if (patient == null) return;

            patient.PatientStatus = patient.LastStatus;
            patient.LastStatus = patient.PatientStatus;

            Repository.Entry(patient).State = EntityState.Modified;
            Save();
        }

        public override void Dispose()
        {
            _nbrSurveillanceService.Dispose();
            _hypothermiaService.Dispose();
            base.Dispose();
        }
    }
}