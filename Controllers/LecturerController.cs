using Microsoft.AspNetCore.Mvc;

namespace CMCS_Web.Controllers
{
    public class LecturerController : Controller
    {
        // GET: /Lecturer/Dashboard
        [HttpGet]
        public IActionResult Dashboard()
        {
            // returns Views/Lecturer/Dashboard.cshtml
            return View();
        }

        // optional helper actions used by dashboard buttons
        [HttpGet]
        public IActionResult SubmitClaim() => RedirectToAction("Create", "Claims");

        [HttpGet]
        public IActionResult ViewMyClaims() => RedirectToAction("Index", "Claims");

        [HttpGet]
        public IActionResult UploadDocument() => RedirectToAction("Create", "Claims");
    }
}
