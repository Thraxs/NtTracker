using System.Net;
using System.Web.Mvc;
using NtTracker.Services;

namespace NtTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IService _service;

        public HomeController(IService service)
        {
            _service = service;
        }

        // GET: /
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Init
        [AllowAnonymous]
        public ActionResult Init()
        {
            //Application warmup
            _service.Initialize();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _service.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}