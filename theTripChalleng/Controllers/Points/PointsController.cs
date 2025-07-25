//create full controller for Points
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using theTripChalleng.Data;
using theTripChalleng.Models;
using theTripChalleng.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using theTripChalleng.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace theTripChalleng.Controllers.Points
{
    public class PointsController : Controller
    {
        private readonly AppDbContext _context;

        public PointsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");
                
            var pointsHistory = _context.PointsHistories
                .Include(ph => ph.User)
                .Include(ph => ph.Criteria)
                .ToList();

            return View(pointsHistory);
        }

        [HttpGet]
        public JsonResult GetCriteriaByCategory(long categoryId)
        {
            var criteria = _context.Criteria
                .Where(c => c.CategoryId == categoryId)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CriteriaName
                }).ToList();

            return Json(criteria);
        }

        [HttpGet]
        public IActionResult RequestPoints()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");
                
            var categories = _context.Categories.ToList();
            var criteria = _context.Criteria.ToList();

            ViewBag.AllowedPoints = Enum.GetValues(typeof(EnumHelper.AllowedPoints))
                .Cast<EnumHelper.AllowedPoints>()
                .Select(p => new SelectListItem
                {
                    Value = ((int)p).ToString(),
                    Text = ((int)p).ToString()
                }).ToList();

            // FIX: Convert categories to SelectListItem
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CategoryName // or whatever property you want to display
            }).ToList();

            ViewBag.Criteria = criteria.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CriteriaName
            }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RequestPoints(PointRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pointRequest = new PointRequest
                {
                    UserId = HttpContext.Session.GetInt32("UserId").Value,
                    CriteriaId = model.CriterionId,
                    RequestedPoints = model.RequestedPoints,
                    Proof = model.Proof,
                    CreatedAt = DateTime.Now,
                    Status = _context.RequestStatuses.FirstOrDefault(s => s.StatusName == "Pending")
                };

                _context.PointRequests.Add(pointRequest);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            var criteria = _context.Criteria.ToList();
            ViewBag.AllowedPoints = Enum.GetValues(typeof(EnumHelper.AllowedPoints))
                .Cast<EnumHelper.AllowedPoints>()
                .Select(p => new SelectListItem
                {
                    Value = ((int)p).ToString(),
                    Text = ((int)p).ToString()
                }).ToList();

            ViewBag.Criteria = criteria.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CriteriaName
            }).ToList();

            return View(model);
        }
    }
}