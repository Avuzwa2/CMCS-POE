using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CMCS_Web.Services;

namespace CMCS_Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IClaimService _svc;
        public AdminController(IClaimService svc) => _svc = svc;

        public async Task<IActionResult> Index()
        {
            var claims = await _svc.GetAllAsync();
            return View(claims);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            await _svc.UpdateStatusAsync(id, "Approved");
            TempData["Message"] = "Claim approved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            await _svc.UpdateStatusAsync(id, "Rejected");
            TempData["Message"] = "Claim rejected.";
            return RedirectToAction(nameof(Index));
        }
    }
}
