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
                    CreatedAt = DateTime.UtcNow,
                    Status = _context.RequestStatuses.FirstOrDefault(s => s.StatusName == "Pending")
                };

                _context.PointRequests.Add(pointRequest);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            var categories = _context.Categories.ToList();
            ViewBag.AllowedPoints = Enum.GetValues(typeof(EnumHelper.AllowedPoints))
                .Cast<EnumHelper.AllowedPoints>()
                .Select(p => new SelectListItem
                {
                    Value = ((int)p).ToString(),
                    Text = ((int)p).ToString()
                }).ToList();

            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CategoryName // or whatever property you want to display
            }).ToList();

            return View(model);
        }

        // add actions from adding points history as the admin gives points to the user
        [HttpGet]
        public IActionResult AddPoints()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var categories = _context.Categories.ToList();
            var criteria = _context.Criteria.ToList();

            var users = _context.Users.ToList();
            ViewBag.Users = users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Name
            }).ToList();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CategoryName
            }).ToList();
            ViewBag.Criterions = criteria.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CriteriaName
            }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPoints(PointsHistoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pointsHistory = new PointsHistory
                {
                    UserId = model.UserId,
                    CriteriaId = model.CriterionId,
                    Points = model.Points,
                    CreatedAt = DateTime.UtcNow,
                    ApprovedBy = HttpContext.Session.GetString("UserName")
                };

                var user = await _context.Users.FindAsync(model.UserId);
                if (user != null)
                {
                    user.TotalPoints += model.Points;
                }

                _context.PointsHistories.Add(pointsHistory);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            var categories = _context.Categories.ToList();
            var users = _context.Users.ToList();
            ViewBag.Users = users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Name
            }).ToList();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CategoryName
            }).ToList();

            return View(model);
        }

        // create action to show request information
        [HttpGet]
        public IActionResult RequestInfo(long requestId)
        {
            var pointRequest = _context.PointRequests
            .Include(pr => pr.User)
            .Include(pr => pr.Criteria)
            .Include(pr => pr.Status)
            .FirstOrDefault(pr => pr.RequestId == requestId);

            if (pointRequest == null)
                return NotFound();

            return View(pointRequest);
        }

        // create action to approve or reject request (the form has admin comments in string to store it in PointRequest with the action "approve" or "reject")
        [HttpPost]
        public async Task<IActionResult> UpdateRequest(long requestId, string action, string comment)
        {
            var pointRequest = await _context.PointRequests.FindAsync(requestId);
            if (pointRequest == null)
                return NotFound();

            pointRequest.AdminComment = comment;

            PointsHistory pointsHistory = null;
            if (action == "approve")
            {
                pointRequest.StatusId = (int)EnumHelper.RequestStatus.Approved;
                pointRequest.AdminComment = comment;
                pointsHistory = new PointsHistory
                {
                    UserId = pointRequest.UserId,
                    CriteriaId = pointRequest.CriteriaId,
                    Points = pointRequest.RequestedPoints,
                    CreatedAt = DateTime.UtcNow,
                    ApprovedBy = HttpContext.Session.GetString("UserName")
                };
                var user = await _context.Users.FindAsync(pointRequest.UserId);
                if (user != null)
                {
                    user.TotalPoints += pointRequest.RequestedPoints;
                }
            }
            else if (action == "reject")
            {
                pointRequest.StatusId = (int)EnumHelper.RequestStatus.Rejected;
                pointRequest.AdminComment = comment;
            }

            _context.PointRequests.Update(pointRequest);
            if (action == "approve" && pointsHistory != null)
            {
                _context.PointsHistories.Add(pointsHistory);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}