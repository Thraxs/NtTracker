using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NtTracker.Models;
using NtTracker.Services;
using NtTracker.ViewModels;

namespace NtTracker.Controllers
{
    [Authorize]
    public class HypothermiaController : Controller
    {
        private readonly IHypothermiaService _hypothermiaService;
        private readonly IPatientService _patientService;

        public HypothermiaController(IHypothermiaService hypothermiaService, IPatientService patientService)
        {
            _hypothermiaService = hypothermiaService;
            _patientService = patientService;
        }

        // GET: Hypothermia/View/5
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var patientId = (int) id;
            var hypothermias = _hypothermiaService.FindByPatientId(patientId);

            var viewModel = new ViewHypothermiaViewModel
            {
                PatientId = (int) id,
                Hypothermias = hypothermias
            };

            return View(viewModel);
        }

        // GET: Hypothermia/Create
        public ActionResult Create(int fromId)
        {
            return View(new EditHypothermiaViewModel());
        }

        // POST: Hypothermia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int fromId, EditHypothermiaViewModel hypothermiaViewModel)
        {
            if (!ModelState.IsValid) return View(hypothermiaViewModel);

            //Check that the tracking is not closed
            if (_patientService.IsClosed(fromId)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            hypothermiaViewModel.ModelState = ModelState;
            _hypothermiaService.Create(hypothermiaViewModel, fromId);

            //Check validation again for repeated time slots
            if (!ModelState.IsValid) return View(hypothermiaViewModel);

            _hypothermiaService.Log(OperationType.HypothermiaCreate, User.Identity.GetUserId<int>(), fromId);
            return RedirectToAction("View", "Hypothermia", new { id = fromId });
        }

        // GET: Hypothermia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var hypothermia = _hypothermiaService.FindById((int)id);
            if (hypothermia == null)
            {
                return HttpNotFound();
            }

            return View(new EditHypothermiaViewModel(hypothermia));
        }

        // POST: Hypothermia/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int fromId, EditHypothermiaViewModel hypothermiaViewModel)
        {
            if (!ModelState.IsValid) return View(hypothermiaViewModel);

            //Check that the tracking is not closed
            if (_patientService.IsClosed(fromId)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var updated = _hypothermiaService.Update(hypothermiaViewModel);

            //Update failed because of a wrong id
            if (!updated) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //Update successful, log operation and redirect to view hypothermias
            _hypothermiaService.Log(OperationType.HypothermiaUpdate, User.Identity.GetUserId<int>(),
                patientId: fromId, data: "HypothermiaID: " + hypothermiaViewModel.Id);
            return RedirectToAction("View", "Hypothermia", new { id = fromId });
        }

        // POST: Hypothermia/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, int fromId)
        {
            //Check that the tracking is not closed
            if (_patientService.IsClosed(fromId)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var deleted = _hypothermiaService.Delete(id);

            //Delete failed because of a wrong id
            if (!deleted) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            _hypothermiaService.Log(OperationType.HypothermiaDelete, User.Identity.GetUserId<int>(),
                patientId: fromId, data: "HypothermiaID: " + id);
            return RedirectToAction("View", "Hypothermia", new { id = fromId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _hypothermiaService.Dispose();
                _patientService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}