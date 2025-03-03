using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartCitizen.Data;
using SmartCitizen.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
[Area("User")]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // This action will be called when accessing the Index page (dashboard).
  
     // Ensure the role is checked for access
    public async Task<IActionResult> Index()
    {
        List<Complaint> complaints = _context.Complaints.ToList();
        return View(complaints); // Pass data to the view
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


    public async Task<IActionResult> Contact()
    {

        return View();
    } 
    public async Task<IActionResult> About()
    {

        return View();
    }
}
