using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using theTripChalleng.Data;
using theTripChalleng.Helpers;
using theTripChalleng.Models;
using theTripChalleng.ViewModels;

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

        //make action to display the rewards the user can get
        public IActionResult Rewards()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var rewards = _context.Rewards.ToList();

            return View(rewards);
        }

        // action to add reward that the user can get
        [HttpGet]
        public IActionResult AddReward()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            //categories dropdown
            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                }).ToList();

            return View();
        }
        [HttpPost]
        public IActionResult AddReward(AddRewardViewModel reward)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var newReward = new Reward
                {
                    Name = reward.Name,
                    Description = reward.Description,
                    MinPoints = reward.MinPoints,
                    CategoryId = reward.CategoryId,
                    Image = reward.ImageFile != null ? FilesHelper.ConvertToBytes(reward.ImageFile) : null
                };

                _context.Rewards.Add(newReward);
                _context.SaveChanges();
                return RedirectToAction("Rewards");
            }

            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                }).ToList();

            return View(reward);
        }

        // action to edit reward
        [HttpGet]
        public IActionResult EditReward(long id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var reward = _context.Rewards.Find(id);
            if (reward == null)
                return NotFound();

            var model = new EditRewardViewModel
            {
                Id = reward.Id,
                Name = reward.Name,
                Description = reward.Description,
                MinPoints = reward.MinPoints,
                CategoryId = reward.CategoryId,
                Image = reward.Image
            };

            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult EditReward(EditRewardViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var reward = _context.Rewards.Find(model.Id);
                if (reward == null)
                    return NotFound();

                reward.Name = model.Name;
                reward.Description = model.Description;
                reward.MinPoints = model.MinPoints ?? reward.MinPoints;
                reward.CategoryId = model.CategoryId ?? reward.CategoryId;

                // if the no image file is selected and the image is deleted, set the image to null
                if (model.IsImageDeleted && model.ImageFile == null)
                {
                    reward.Image = null;
                }else if (model.ImageFile != null)
                {
                    // Convert the uploaded image file to bytes
                    reward.Image = FilesHelper.ConvertToBytes(model.ImageFile);
                }
                
                _context.Rewards.Update(reward);
                _context.SaveChanges();
                return RedirectToAction("Rewards");
            }

            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                }).ToList();

            return View(model);
        }

    }
}