// create a full controller for Account
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using theTripChalleng.Data;
using theTripChalleng.Models;
using theTripChalleng.ViewModel;

// Add the following line if LoginViewModel is in a different namespace 
using theTripChalleng.ViewModels;

namespace theTripChalleng.Controllers.Home
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Check if the user is already logged in
            // If so, redirect to the home page
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var user = _context.Users
                    .Include(u => u.Rule)
                    .FirstOrDefault(u => u.Phone == model.Phone && u.Password == model.Password);
                    if (user != null)
                    {
                        HttpContext.Session.SetInt32("UserId", (int)user.Id);
                        HttpContext.Session.SetString("UserName", user.Name);
                        HttpContext.Session.SetString("UserRole", user.Rule?.RuleName ?? "User");
                        HttpContext.Session.SetInt32("UserPoints", (int)(user.TotalPoints ?? 0));
                        HttpContext.Session.SetString("UserImage", user.Image != null ? Convert.ToBase64String(user.Image) : string.Empty);
                        // store the user rank by getting his points and sorting the users by points
                        var users = _context.Users.ToList();
                        var userRank = users.OrderByDescending(u => u.TotalPoints).ToList().IndexOf(user) + 1;
                        HttpContext.Session.SetInt32("UserRank", userRank);
                        // TODO: Implement authentication
                        return RedirectToAction("Details", "Account");
                    }
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) if necessary
                ModelState.AddModelError("", "An error occurred while processing your request.");
                ViewData["ErrorMessage"] = "Database connection error: " + ex.Message;
                //redirect to login page
                return RedirectToAction("Login");
            }
        }

        // Details action using userId from session
        [SessionAuthorize]
        [HttpGet]
        public IActionResult Details()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            // ✅ Load user with related data (only what we really need)
            var user = _context.Users
                .Include(u => u.Rule)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound();

            // ✅ Load related data separately (simpler logic)
            var pointsHistory = _context.PointsHistories
                .Include(ph => ph.Criteria)
                .Where(ph => ph.UserId == userId)
                .ToList();


            ViewBag.PendingAssignedCriteria = _context.AssignedCriteras
            .Where(ac => ac.UserId == userId)
            .OrderByDescending(ac => ac.CreatedAt)
            .ToList() // Materialize first
            .Select(ac => new AssignedCriteriaViewModel
            {
                AssignedCriterias = ac,
                User = _context.Users.Find(ac.UserId),
                Criteria = _context.Criteria.Find(ac.CriteriaId)
            })
            .ToList();


            var pointsRequests = _context.PointRequests
                .Include(pr => pr.Criteria)
                .Include(pr => pr.Status)
                .Where(pr => pr.UserId == userId)
                .ToList();

            // get value of TotalItemsCount (total number of criterias in the DB) and RegisteredItemsCount (total number of criterias recorded for the user in history table)
            var totalItemsCount = _context.Criteria.Count();

            // Get total number of criterias recorded for the user in history table (distinct by CriteriaId)
            var registeredItemsCount = _context.PointsHistories
                .Where(ph => ph.UserId == userId && ph.CriteriaId != null)
                .Select(ph => ph.CriteriaId)
                .Distinct()
                .Count();

            // ✅ Now just map to ViewModels (values already loaded)
            var historyVM = pointsHistory.Select(ph => new PointsHistoryViewModel
            {
                Id = ph.Id,
                UserId = ph.UserId ?? 0,
                UserName = user.Name, // already loaded user
                CriterionId = ph.CriteriaId ?? 0,
                CriterionName = ph.Criteria?.CriteriaName,
                Points = ph.Points ?? 0,
                CreatedAt = ph.CreatedAt
            }).ToList();

            var requestsVM = pointsRequests.Select(pr => new PointRequestViewModel
            {
                Id = pr.RequestId,
                UserId = pr.UserId,
                UserName = user.Name, // already loaded user
                CriterionId = pr.CriteriaId ?? 0,
                CriterionName = pr.Criteria?.CriteriaName,
                RequestedPoints = pr.RequestedPoints ?? 0,
                Proof = pr.Proof,
                AdminComment = pr.AdminComment,
                CreatedAt = pr.CreatedAt,
                StatusName = pr.Status?.StatusName
            }).ToList();

            var model = new UserDetailsViewModel
            {
                User = user,
                PointsHistory = historyVM,
                PointsRequests = requestsVM,
                RoleName = user.Rule?.RuleName ?? string.Empty,
                TotalItemsCount = totalItemsCount,
                RegisteredItemsCount = registeredItemsCount
            };

            return View(model);
        }



        //edit action
        [HttpGet]
        public IActionResult Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.Find((long)userId);
            if (user == null)
            {
                return NotFound();
            }

            // Map User to EditProfileViewModel
            var model = new EditProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Phone = user.Phone,
                Password = user.Password,
                Image = user.Image
                // Add other properties as needed
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditProfileViewModel user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.Find((long)user.Id);
                if (existingUser == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View(user);
                }

                // if the field is not null, update it
                if (!string.IsNullOrEmpty(user.Name))
                {
                    existingUser.Name = user.Name;
                }

                if (!string.IsNullOrEmpty(user.Phone))
                {
                    existingUser.Phone = user.Phone;
                }

                if (!string.IsNullOrEmpty(user.Password))
                {
                    existingUser.Password = user.Password; // Consider hashing the password
                }

                if (!string.IsNullOrEmpty(user.ConfirmPassword))
                {
                    if (user.Password != user.ConfirmPassword)
                    {
                        ModelState.AddModelError("", "Passwords do not match.");
                        return View(user);
                    }
                }

                if (user.IsImageDeleted && user.ImageFile == null)
                {
                    existingUser.Image = null;
                }
                else if (user.ImageFile != null)
                {
                    const long maxFileSize = 2 * 1024 * 1024; // 2 MB
                    if (user.ImageFile.Length > maxFileSize)
                    {
                        ModelState.AddModelError("ImageFile", "يجب أن يكون حجم الصورة أقل من 2 ميجابايت.");
                        return View(user);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        user.ImageFile.CopyTo(memoryStream);
                        existingUser.Image = memoryStream.ToArray();
                    }
                }

                _context.Users.Update(existingUser);
                // Save changes to the database
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }

        // Register action
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (model.ImageFile != null)
            {
                const long maxFileSize = 2 * 1024 * 1024; // 2 MB
                if (model.ImageFile.Length > maxFileSize)
                {
                    ModelState.AddModelError("ImageFile", "يجب أن يكون حجم الصورة أقل من 2 ميجابايت.");
                    return View(model);
                }

                using (var memoryStream = new MemoryStream())
                {
                    model.ImageFile.CopyTo(memoryStream);
                    model.Image = memoryStream.ToArray();
                }
            }
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Name = model.Name ?? string.Empty,
                    Phone = model.Phone ?? string.Empty,
                    Password = model.Password,
                    Image = model.Image,
                    RuleId = 2,
                    TotalPoints = 0,
                    CreatedAt = DateTime.Now
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpGet]
        public JsonResult IsPhoneAvailable(string phone)
        {
            bool exists = _context.Users.Any(u => u.Phone == phone);
            return Json(!exists); // true if available, false if taken
        }

        // Logout action
        [SessionAuthorize]
        [HttpGet]
        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
