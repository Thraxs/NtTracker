using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NtTracker.Documents;
using NtTracker.Models;
using NtTracker.Resources.Patient;
using NtTracker.Services;
using NtTracker.ViewModels;

namespace NtTracker.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        const int PageSize = 10;
        const int ExportSize = 100;

        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // GET: Patient/List
        public ActionResult List(string pid, string nhc, string nuhsa, string name, string surnames, 
            PatientStatus? patientStatus, string birthFrom, string birthTo, string sorting, int? page)
        {
            //Sorting
            ViewBag.IdSort = sorting == "id_a" ? "id_d" : "id_a";
            ViewBag.NhcSort = sorting == "nhc_a" ? "nhc_d" : "nhc_a";
            ViewBag.NuhsaSort = sorting == "nuhsa_a" ? "nuhsa_d" : "nuhsa_a";
            ViewBag.NameSort = sorting == "name_a" ? "name_d" : "name_a";
            ViewBag.SurnameSort = sorting == "surnames_a" ? "surnames_d" : "surnames_a";
            ViewBag.BirthSort = sorting == "birthdate_a" ? "birthdate_d" : "birthdate_a";
            ViewBag.StatusSort = sorting == "status_a" ? "status_d" : "status_a";

            //Pagination
            var pageNumber = (page ?? 1);

            var patients = _patientService.Search(pid, nhc, nuhsa, name, surnames, 
                patientStatus, birthFrom, birthTo, sorting, pageNumber, PageSize);

            return View(patients);
        }

        // GET: Patient/Export
        public FileStreamResult Export(string pid, string nhc, string nuhsa, string name, string surnames,
            PatientStatus? patientStatus, string birthFrom, string birthTo, string sorting)
        {
            var patientData = _patientService.DeepSearch(pid, nhc, nuhsa, name, surnames,
                patientStatus, birthFrom, birthTo, sorting, 1, ExportSize);

            var dataStream = SpreadsheetManager.ExportPatientData(patientData);

            return File(dataStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Strings.ExportedFileName + ".xlsx");
        }

        // GET: Patient/View/5
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var patient = _patientService.FindById((int) id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(new PatientViewModel(patient));
        }

        // GET: Patient/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Patient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PatientViewModel patient)
        {
            if (!ModelState.IsValid) return View(patient);

            //Get current user credentials to store as creator of the patient
            var registrant = User.Identity.GetUserName();
            var registrantId = User.Identity.GetUserId<int>();

            var newPatient = _patientService.Create(patient, registrant, registrantId);
            _patientService.Log(OperationType.PatientCreate, registrantId, patientId: newPatient);

            return RedirectToAction("List");
        }

        // GET: Patient/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var patient = _patientService.FindById((int) id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(new PatientViewModel(patient));
        }

        // POST: Patient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PatientViewModel patient)
        {
            if (!ModelState.IsValid) return View(patient);

            //Check that the tracking is not closed
            if (_patientService.IsClosed(patient.Id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var updated = _patientService.Update(patient);

            //Update failed because of a wrong id
            if (!updated) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //Update successful, log operation and redirect to view patient
            _patientService.Log(OperationType.PatientUpdate, User.Identity.GetUserId<int>(), patient.Id);
            return RedirectToAction("View", new { patient.Id });
        }

        // POST: Patient/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            //Check that the tracking is not closed
            if (_patientService.IsClosed(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var deleted = _patientService.Delete(id);

            //Delete failed because of a wrong id
            if (!deleted) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            _patientService.Log(OperationType.PatientDelete, User.Identity.GetUserId<int>(), data: "PatientID: " + id);
            return RedirectToAction("List");
        }

        // POST: Patient/Close/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Close(int id)
        {
            _patientService.ClosePatientTracking(id);

            _patientService.Log(OperationType.PatientClose, User.Identity.GetUserId<int>(), patientId: id);
            return RedirectToAction("View", new { id });
        }

        // POST: Patient/Open/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Open(int id)
        {
            _patientService.OpenPatientTracking(id);

            _patientService.Log(OperationType.PatientOpen, User.Identity.GetUserId<int>(), patientId: id);
            return RedirectToAction("View", new { id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _patientService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
