using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NtTracker.Models;
using NtTracker.Services;
using NtTracker.ViewModels;

namespace NtTracker.Controllers
{
    [Authorize]
    public class NbrSurveillanceController : Controller
    {
        private readonly INbrSurveillanceService _nbrSurveillanceService;
        private readonly IPatientService _patientService;

        public NbrSurveillanceController(INbrSurveillanceService nbrSurveillanceService, IPatientService patientService)
        {
            _nbrSurveillanceService = nbrSurveillanceService;
            _patientService = patientService;
        }

        // GET: NbrSurveillance/View/5
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var patientId = (int)id;
            var patientStatus = _patientService.GetStatus(patientId);
            var surveillances = _nbrSurveillanceService.FindByPatientId(patientId);

            var viewModel = new ViewNbrSurveillanceViewModel
            {
                PatientId = (int)id,
                PatientStatus = patientStatus,
                NbrSurveillances = surveillances
            };

            return View(viewModel);
        }

        // GET: NbrSurveillance/Create
        public ActionResult Create(int fromId)
        {
            return View(new EditNbrSurveillanceViewModel());
        }

        // POST: NbrSurveillance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int fromId, EditNbrSurveillanceViewModel nbrSurveillanceViewModel)
        {
            if (!ModelState.IsValid) return View(nbrSurveillanceViewModel);

            //Check that the tracking is not closed
            if (_patientService.IsClosed(fromId)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            nbrSurveillanceViewModel.ModelState = ModelState;
            _nbrSurveillanceService.Create(nbrSurveillanceViewModel, fromId);

            //Check validation again for repeated time slots
            if (!ModelState.IsValid) return View(nbrSurveillanceViewModel);

            //Check if patient status needs to be updated
            _patientService.UpdateStatus(fromId);

            _nbrSurveillanceService.Log(OperationType.NbrSurveillanceCreate, User.Identity.GetUserId<int>(), fromId);
            return RedirectToAction("View", "NbrSurveillance", new { id = fromId });
        }

        // GET: NbrSurveillance/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var surveillance = _nbrSurveillanceService.FindById((int)id);
            if (surveillance == null)
            {
                return HttpNotFound();
            }

            return View(new EditNbrSurveillanceViewModel(surveillance));
        }

        // POST: NbrSurveillance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int fromId, EditNbrSurveillanceViewModel nbrSurveillance)
        {
            if (!ModelState.IsValid) return View(nbrSurveillance);

            //Check that the tracking is not closed
            if (_patientService.IsClosed(nbrSurveillance.Id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var updated = _nbrSurveillanceService.Update(nbrSurveillance);

            //Update failed because of a wrong id
            if (!updated) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //Check if patient status needs to be updated
            _patientService.UpdateStatus(fromId);

            //Update successful, log operation and redirect to view nbr surveillances
            _nbrSurveillanceService.Log(OperationType.NbrSurveillanceUpdate, User.Identity.GetUserId<int>(), nbrSurveillance.Id);
            return RedirectToAction("View", new { id = fromId });
        }

        // POST: NbrSurveillance/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, int fromId)
        {
            //Check that the tracking is not closed
            if (_patientService.IsClosed(fromId)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var deleted = _nbrSurveillanceService.Delete(id);

            //Delete failed because of a wrong id
            if (!deleted) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            _nbrSurveillanceService.Log(OperationType.NbrSurveillanceDelete, User.Identity.GetUserId<int>(),
                patientId: fromId, data: "NbrSurveillanceID: " + id);
            return RedirectToAction("View", "NbrSurveillance", new { id = fromId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _nbrSurveillanceService.Dispose();
                _patientService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}