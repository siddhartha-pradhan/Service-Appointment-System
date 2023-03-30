using Microsoft.AspNetCore.Mvc;

namespace ServiceAppointmentSystem.Areas.Admin.Controllers
{
    public class ApprovalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
