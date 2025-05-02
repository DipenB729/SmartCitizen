using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCitizen.Data;
using SmartCitizen.Models;

namespace SmartCitizen.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            List<Complaint> complaints = _context.Complaints.ToList();
            return View(complaints);
        }
        public async Task<IActionResult> Upsert(int id)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null)
            {
                return NotFound();
            }

            return View(complaint);
        }

        [HttpPost]
        public IActionResult Upsert(Complaint complaint)
        {
            if (ModelState.IsValid)
            {
                var existingComplaint = _context.Complaints.FirstOrDefault(c => c.Id == complaint.Id);
                if (existingComplaint != null)
                {
                    existingComplaint.Status = complaint.Status; // Update only the status
                    _context.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            return View(complaint);
        }


        public IActionResult AddComment(int id)
        {
            var complaint = _context.Complaints
                .Include(c => c.Comments)  // Include related comments
                .ThenInclude(comment => comment.User)  // If you need user details (e.g., name)
                .FirstOrDefault(c => c.Id == id);

            if (complaint == null)
            {
                return NotFound();
            }

            return View(complaint);
        }
        [HttpPost]
        [Authorize]
        public IActionResult AddComment(int complaintId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Comment cannot be empty.";
                return RedirectToAction("AddComment", new { complaintId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user ID

            var comment = new Comment
            {
                ComplaintId = complaintId,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _context.Comment.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("AddComment", new { complaintId });
        }
        [HttpPost]
        public IActionResult DeleteComment(int commentId, int complaintId)
        {
            var comment = _context.Comment.FirstOrDefault(c => c.Id == commentId);

            if (comment == null)
            {
                return NotFound();
            }

            // Ensure only the owner of the comment can delete it
            if (User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value != comment.UserId)
            {
                return Forbid(); // Unauthorized access
            }

            _context.Comment.Remove(comment);
            _context.SaveChanges();

            // Redirect back to the complaint details page
            return RedirectToAction("AddComment", new { id = complaintId });
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
