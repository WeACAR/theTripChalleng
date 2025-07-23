using Microsoft.AspNetCore.Mvc;
using theTripChalleng.Data;

namespace theTripChalleng.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "MVC";
            var data = _context.Users.ToList(); // Assuming you have a DbSet<User> in your AppDbContext
            return View(data);
        }
    }
}