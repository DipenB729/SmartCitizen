using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartCitizen.Models;

using SmartCitizen.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Identity;


namespace SmartCitizen.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = SD.Role_PublicUser)]
    public class ComplaintController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;



        public ComplaintController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
          
        }

        public IActionResult Index()
        {
           
            List<Complaint> complaints = _context.Complaints.ToList();
            return View(complaints);
        }

        public IActionResult Upsert(int id)
        {
            Complaint complaint = new();
            if (id == null || id == 0)
            {
                return View(complaint);
            }
            else
            {
                complaint = _context.Complaints.Find(id);
                return View(complaint);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Complaint complaint, IFormFile? file)
        {
            string userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // If UserId is null, return Unauthorized response
            }

            // Assign UserId to the complaint before validation
            complaint.UserId = userId;
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string complaintPath = Path.Combine(wwwRootPath, "images/complaint");
                    if (!string.IsNullOrEmpty(complaint.ImagePath))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, complaint.ImagePath.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(complaintPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    complaint.ImagePath = @"\images\complaint\" + fileName;  // Add a leading slash

                }

                if (complaint.Id == 0)
                {
                    _context.Complaints.Add(complaint);
                }
                else
                {
                    _context.Complaints.Update(complaint);
                }

                _context.SaveChanges();
                TempData["success"] = "Complaint submitted successfully";
                return RedirectToAction("Index", "Home");
            }
            return View(complaint);
        }
        public IActionResult Show(int id)
        {
            var complaint = _context.Complaints.Find(id);
            if (complaint == null)
            {
                return NotFound();
            }
            return View(complaint);
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Complaint> complaints = _context.Complaints.ToList();
            return Json(new { data = complaints });
        }

        // Prevent CSRF attacks
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var complaintToDelete = _context.Complaints.Find(id);
            if (complaintToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Delete Image if Exists
            if (!string.IsNullOrEmpty(complaintToDelete.ImagePath))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, complaintToDelete.ImagePath.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _context.Complaints.Remove(complaintToDelete);
            _context.SaveChanges();

            // Redirect after deletion (if needed)
            return RedirectToAction("Index"); // Redirect to index or another view after deletion
        }

        #endregion
    }
}
