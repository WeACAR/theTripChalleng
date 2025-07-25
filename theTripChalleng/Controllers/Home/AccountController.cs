// create a full controller for Account
using Microsoft.AspNetCore.Mvc;
using theTripChalleng.Data;
using theTripChalleng.Models;
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
                    var user = _context.Users.FirstOrDefault(u => u.Phone == model.Phone && u.Password == model.Password);
                    if (user != null)
                    {
                        HttpContext.Session.SetInt32("UserId", (int)user.Id);
                        // TODO: Implement authentication
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) if necessary
                ModelState.AddModelError("", "An error occurred while processing your request.");
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
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.Find((long)userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
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
                    return NotFound();
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

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Name = model.Name,
                    Phone = model.Phone,
                    Password = model.Password,
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

        [HttpPost]
        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
