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

        // This action is protected by the SessionAuthorize attribute
        [SessionAuthorize]
        public IActionResult Index()
        {
            //check session and redirect
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewData["Title"] = "MVC";
            var data = _context.Users.ToList(); // Assuming you have a DbSet<User> in your AppDbContext
            return View(data);
        }
    }
}