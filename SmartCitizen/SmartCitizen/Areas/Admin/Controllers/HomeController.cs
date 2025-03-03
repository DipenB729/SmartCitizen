using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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


        public IActionResult Show(int id)
        {
            var complaint = _context.Complaints.Find(id);
            if (complaint == null)
            {
                return NotFound();
            }
            return View(complaint);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
