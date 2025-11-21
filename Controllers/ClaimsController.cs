using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CMCS_Web.Models;
using CMCS_Web.Services;
using Microsoft.Extensions.Hosting;

namespace CMCS_Web.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IClaimService _svc;
        private readonly IWebHostEnvironment _env;

        public ClaimsController(IClaimService svc, IWebHostEnvironment env)
        {
            _svc = svc;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await _svc.GetAllAsync();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Claim());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim claim, IFormFile? uploadedFile)
        {
            if (!ModelState.IsValid)
                return View(claim);

            // File handling
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                var allowed = new[] { ".pdf", ".doc", ".docx", ".xlsx" };
                var ext = Path.GetExtension(uploadedFile.FileName).ToLowerInvariant();
                if (Array.IndexOf(allowed, ext) < 0)
                {
                    ModelState.AddModelError("uploadedFile", "File type not allowed.");
                    return View(claim);
                }

                // limit size to 6 MB
                if (uploadedFile.Length > 6 * 1024 * 1024)
                {
                    ModelState.AddModelError("uploadedFile", "File too large (max 6 MB).");
                    return View(claim);
                }

                var uploadsFolder = Path.Combine(_env.WebRootPath ?? ".", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var safeName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, safeName);
                await using var fs = System.IO.File.Create(filePath);
                await uploadedFile.CopyToAsync(fs);
                claim.UploadedFileName = safeName;
            }

            claim.Status = "Pending";
            claim.DateSubmitted = DateTime.UtcNow;

            // Use the service CreateAsync (we added this to the interface above)
            await _svc.CreateAsync(claim);

            TempData["Message"] = "Claim submitted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // Download supporting file
        [HttpGet]
        public IActionResult Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return NotFound();

            var path = Path.Combine(_env.WebRootPath ?? ".", "uploads", fileName);
            if (!System.IO.File.Exists(path)) return NotFound();

            var content = System.IO.File.ReadAllBytes(path);
            var ext = Path.GetExtension(path).ToLowerInvariant();
            var contentType = ext switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };

            return File(content, contentType, fileName);
        }
    }
}
