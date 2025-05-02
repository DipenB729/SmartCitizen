using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCitizen.Data;
using SmartCitizen.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SmartCitizen.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ComplaintController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;  // Use ApplicationUser here
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ComplaintController(ApplicationDbContext db, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            // Get the logged-in user (ApplicationUser)
            var user = await _userManager.GetUserAsync(User);  // Fetch the ApplicationUser
            var complaints = await _db.Complaints.Where(c => c.UserId == user.Id).ToListAsync();
            return View(complaints);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Complaint complaint = new();
            var user = await _userManager.GetUserAsync(User);  // Fetch ApplicationUser here

            if (id == null || id == 0)
            {
                complaint.UserId = user.Id; // Set the UserId from ApplicationUser
                return View(complaint);
            }
            else
            {
                complaint = await _db.Complaints.FindAsync(id);
                if (complaint == null)
                {
                    return NotFound();
                }

                complaint.UserId = user.Id; // Ensure the UserId is set correctly
                return View(complaint);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Complaint complaint, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);  // Fetch ApplicationUser here
                complaint.UserId = user.Id;

                // Handle file upload for ImagePath
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string uploads = Path.Combine(_webHostEnvironment.WebRootPath, "images/complaints");
                    string filePath = Path.Combine(uploads, fileName);

                    if (!string.IsNullOrEmpty(complaint.ImagePath))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, complaint.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    complaint.ImagePath = "/images/complaints/" + fileName;
                }

                if (complaint.Id == 0)
                {
                    _db.Complaints.Add(complaint);
                }
                else
                {
                    _db.Complaints.Update(complaint);
                }

                await _db.SaveChangesAsync();
                TempData["success"] = "Complaint submitted successfully!";
                return RedirectToAction("Index");
            }

            TempData["error"] = "There was an error with your submission!";
            return View(complaint);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var complaint = await _db.Complaints.FindAsync(id);
            if (complaint == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            if (!string.IsNullOrEmpty(complaint.ImagePath))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, complaint.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _db.Complaints.Remove(complaint);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted successfully" });
        }
    }
}
