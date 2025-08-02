using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            // viewbag for pending requestPoints and assigned criteria not approved yet
            ViewBag.PendingAssignedCriteriaCount = _context.AssignedCriteras
                .Where(ac => ac.IsAdminAction == false)
                .Count();
            ViewBag.PendingRequestsCount = _context.PointRequests
                .Where(pr => pr.StatusId == (int)EnumHelper.RequestStatus.Pending)
                .Count();

            // make it show both pending requests and assigned criteria that not approved yet in one ViewBag
            // list the viewmodel AssignedCriteriaViewModel
            ViewBag.PendingAssignedCriteria = _context.AssignedCriteras
            .Where(ac => ac.IsAdminAction == false)
            .OrderByDescending(ac => ac.CreatedAt)
            .ToList() // Materialize first
            .Select(ac => new AssignedCriteriaViewModel
            {
                AssignedCriterias = ac,
                User = _context.Users.Find(ac.UserId),
                Criteria = _context.Criteria.Find(ac.CriteriaId)
            })
            .ToList();

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
            var data = _context.Users.ToList();

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");
            var assignedCriteriaIds = _context.AssignedCriteras
                .Where(ac => ac.UserId == userId)
                .Select(ac => ac.CriteriaId)
                .ToList();
            var criterias = _context.Criteria.Where(c => c.IsAssignable == true && !assignedCriteriaIds.Contains(c.Id)).ToList();
            var viewModel = new HomeViewModel
            {
                Leaderboard = data,
                Criterias = criterias
            };

            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CategoryName
            }).ToList();

            return View(viewModel);
        }

        //make an action to display users leaderboard by category (view) where it gets all users for all categories (check if the user have history in the categories) then inside the view it will filter by category
        [HttpGet]
        public IActionResult LeaderboardByCategory()
        {
            var categories = _context.Categories.ToList();

            var leaderboard = categories.Select(category => new
            {
                CategoryId = category.Id,
                CategoryName = category.CategoryName,
                Users = _context.Users
                    .Include(u => u.PointsHistories)
                    .Where(u => u.PointsHistories.Any(ph => ph.Criteria != null && ph.Criteria.CategoryId == category.Id))
                    .Select(u => new
                    {
                        UserId = u.Id,
                        UserName = u.Name,
                        UserImage = u.Image,
                        TotalPoints = u.PointsHistories
                            .Where(ph => ph.Criteria != null && ph.Criteria.CategoryId == category.Id)
                            .Sum(ph => ph.Points)
                    })
                    .OrderByDescending(u => u.TotalPoints)
                    .ToList()
            }).ToList();

            return View(leaderboard);
        }

        //this action should return a list to be displayed in the Index View
        [HttpGet]
        public IActionResult GetUsersByCategory(long categoryId)
        {
            var users = _context.Users
                .Include(u => u.PointsHistories)
                .Where(u => u.PointsHistories.Any(ph => ph.Criteria != null && ph.Criteria.CategoryId == categoryId))
                .ToList();

            var userPoints = users.Select(u => new
            {
                UserId = u.Id,
                UserName = u.Name,
                UserImage = u.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(u.Image)}" : "default-image.png",
                TotalPoints = u.PointsHistories
                    .Where(ph => ph.Criteria?.CategoryId == categoryId)
                    .Sum(ph => ph.Points) ?? 0
            });

            return Json(userPoints);
        }

        //make action to display the rewards the user can get
        public IActionResult Rewards()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // use viewModel RewardDetailViewModel
            var rewards = _context.Rewards
                 .ToList() // Materialize the query first
                 .Select(r => new RewardDetailViewModel
                 {
                     Reward = r,
                     Category = _context.Categories.Find(r.CategoryId)
                 }).ToList();

            return View(rewards);
        }

        //action to view the details of a specific reward
        public IActionResult RewardDetail(long id)
        {
            // use viewModel RewardDetailViewModel 
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var reward = _context.Rewards.Find(id);
            if (reward == null)
                return NotFound();

            var category = _context.Categories.Find(reward.CategoryId);

            var viewModel = new RewardDetailViewModel
            {
                Reward = reward,
                Category = category
            };

            return View(viewModel);
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
                }
                else if (model.ImageFile != null)
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

        // action to add record to AssignedCriteria named AssignCriteria
        [HttpGet]
        public IActionResult AssignCriteria(long criteriaId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var criteria = _context.Criteria.Find(criteriaId);
            if (criteria == null)
            {
                ModelState.AddModelError("", "Criteria not found.");
                return RedirectToAction("Index");
            }
            if (criteria.AssignLeft < 0)
            {
                ModelState.AddModelError("", "No more assignments left for this criteria.");
                return RedirectToAction("Index");
            }
            var assignedCriteria = new AssignedCritera
            {
                UserId = (long)userId,
                CriteriaId = criteriaId,
                CreatedAt = DateTime.UtcNow,
                Approved = false,
                IsAdminAction = false
            };

            criteria.AssignLeft -= 1;

            _context.Criteria.Update(criteria);
            _context.AssignedCriteras.Add(assignedCriteria);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // action to update AssignedCriteria
        [HttpGet]
        public IActionResult UpdateAssignedCriteria(long assignedId, string operation)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var assignedCriteria = _context.AssignedCriteras.Find(assignedId);
            if (assignedCriteria == null)
                return NotFound();

            if (operation == "approve")
            {
                assignedCriteria.Approved = true;
                assignedCriteria.IsAdminAction = true;
            }
            else if (operation == "reject")
            {
                assignedCriteria.Approved = false;
                assignedCriteria.IsAdminAction = true;

                //add to the criteria assign left
                var criteria = _context.Criteria.Find(assignedCriteria.CriteriaId);
                if (criteria != null)
                {
                    criteria.AssignLeft += 1;
                    _context.Criteria.Update(criteria);
                }
            }

            _context.AssignedCriteras.Update(assignedCriteria);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}