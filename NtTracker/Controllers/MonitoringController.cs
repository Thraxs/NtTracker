using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NtTracker.Models;
using NtTracker.Services;
using NtTracker.ViewModels;

namespace NtTracker.Controllers
{
    [Authorize]
    public class MonitoringController : Controller
    {
        private readonly IMonitoringService _monitoringService;
        private readonly INbrSurveillanceService _nbrSurveillanceService;
        private readonly IPatientService _patientService;

        public MonitoringController(IMonitoringService monitoringService, 
            INbrSurveillanceService nbrSurveillanceService, IPatientService patientService)
        {
            _monitoringService = monitoringService;
            _nbrSurveillanceService = nbrSurveillanceService;
            _patientService = patientService;
        }

        // GET: Monitoring/List/5
        public ActionResult List(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var monitorings = _monitoringService.FindByPatientId((int)id);

            var viewModel = new ListMonitoringViewModel
            {
                PatientId = (int)id,
                Monitorings = monitorings
            };

            return View(viewModel);
        }

        // GET: Monitoring/View/5
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var monitoring = _monitoringService.FindById((int)id);
            if (monitoring == null)
            {
                return HttpNotFound();
            }

            return View(new MonitoringViewModel(monitoring));
        }

        // GET: Monitoring/Create
        public ActionResult Create(int fromId)
        {
            return View();
        }

        // POST: Monitoring/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int fromId, MonitoringViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            //Check that the tracking is not closed
            if (_patientService.IsClosed(fromId)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            _monitoringService.Create(viewModel, fromId);

            _monitoringService.Log(OperationType.MonitoringCreate, User.Identity.GetUserId<int>(), fromId);
            return RedirectToAction("List", new { id = fromId });
        }

        // GET: Monitoring/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var monitoring = _monitoringService.FindById((int)id);
            if (monitoring == null)
            {
                return HttpNotFound();
            }
            return View(new MonitoringViewModel(monitoring));
        }

        // POST: Monitoring/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int fromId, MonitoringViewModel monitoringViewModel)
        {
            if (!ModelState.IsValid) return View(monitoringViewModel);

            //Check that the tracking is not closed
            if (_patientService.IsClosed(fromId)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var updated = _monitoringService.Update(monitoringViewModel);

            //Update failed because of a wrong id
            if (!updated) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //Update successful, log operation and redirect to view Monitoring
            _monitoringService.Log(OperationType.MonitoringUpdate, User.Identity.GetUserId<int>(), 
                patientId: fromId, data: "MonitoringId: " + monitoringViewModel.Id);
            return RedirectToAction("List", "Monitoring", new { id = fromId });
        }

        // POST: Monitoring/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, int fromId)
        {
            //Check that the tracking is not closed
            if (_patientService.IsClosed(fromId)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var deleted = _monitoringService.Delete(id);

            //Delete failed because of a wrong id
            if (!deleted) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            _monitoringService.Log(OperationType.MonitoringDelete, User.Identity.GetUserId<int>(),
                patientId: fromId, data: "MonitoringID: " + id);
            return RedirectToAction("List", "Monitoring", new { id = fromId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _monitoringService.Dispose();
                _nbrSurveillanceService.Dispose();
                _patientService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}