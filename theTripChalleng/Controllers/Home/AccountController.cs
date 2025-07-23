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
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Phone == model.Phone && u.Password == model.Password);
                if (user != null)
                {
                    // TODO: Implement authentication
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Phone = model.Phone,
                    Password = model.Password
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(model);
        }
    }
}
