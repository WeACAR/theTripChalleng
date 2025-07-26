using Microsoft.AspNetCore.Mvc;
using theTripChalleng.Data;
using theTripChalleng.Helpers;

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
            // viewbag for pending requestPoints
            ViewBag.PendingRequestsCount = _context.PointRequests
                .Where(pr => pr.StatusId == (int)EnumHelper.RequestStatus.Pending)
                .Count();

            // ViewBag.PendingRequests for pending points requests
            ViewBag.PendingRequests = _context.PointRequests
                .Where(pr => pr.StatusId == (int)EnumHelper.RequestStatus.Pending)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToList();
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