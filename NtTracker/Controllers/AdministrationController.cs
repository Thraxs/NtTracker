using System.Web.Mvc;

namespace NtTracker.Controllers
{
    public class AdministrationController : Controller
    {
        // GET: Administration/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }
    }
}