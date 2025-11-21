using Microsoft.AspNetCore.Mvc;

namespace CMCS_Web.Controllers
{
    public class CoordinatorController : Controller
    {
        // GET: /Coordinator/Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
