using System.Web.Mvc;
using NtTracker.Models;
using NtTracker.Services;

namespace NtTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OperationController : Controller
    {
        const int PageSize = 20;

        private readonly IOperationService _operationService;

        public OperationController(IOperationService operationService)
        {
            _operationService = operationService;
        }

        // GET: Operation/List
        public ActionResult List(string user, OperationType? operation, int? patient,
            string dateFrom, string dateTo, string operationData, string sorting, int? page)
        {
            //Sorting
            ViewBag.TimeSort = sorting == "time_a" ? "time_d" : "time_a";
            ViewBag.UserSort = sorting == "user_a" ? "user_d" : "user_a";
            ViewBag.OperationSort = sorting == "operation_a" ? "operation_d" : "operation_a";
            ViewBag.PatientSort = sorting == "patient_a" ? "patient_d" : "patient_a";

            //Pagination
            var pageNumber = (page ?? 1);

            var operations = _operationService.Search(user, operation, patient, dateFrom, dateTo,
                operationData, sorting, pageNumber, PageSize);

            return View(operations);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _operationService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}