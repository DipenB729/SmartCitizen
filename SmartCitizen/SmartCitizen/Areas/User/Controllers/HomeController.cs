using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartCitizen.Data;
using SmartCitizen.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> Contact()
    {

        return View();
    } 
    public async Task<IActionResult> About()
    {

        return View();
    }
}
