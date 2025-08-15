using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using theTripChalleng.Data;
using theTripChalleng.Models;
using theTripChalleng.Helpers;
using System.Security.Claims;

namespace theTripChalleng.Controllers.Email
{
    public class EmailController : Controller
    {
        private readonly AppDbContext _context;

        public EmailController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Email
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                ViewData["UserRole"] = userRole;

                IQueryable<Models.Email> emails;

                if (userRole == EnumHelper.UserRole.Admin)
                {
                    // Admin can view all emails
                    emails = _context.Emails
                        .Include(e => e.SenderNavigation)
                        .Include(e => e.ReceiverNavigation);
                }
                else
                {
                    // User and Nickname can only view their own emails (sent or received)
                    emails = _context.Emails
                        .Where(e => e.Sender == currentUserId || e.Receiver == currentUserId)
                        .Include(e => e.SenderNavigation)
                        .Include(e => e.ReceiverNavigation);
                }

                var emailList = await emails.OrderByDescending(e => e.CreatedAt).ToListAsync();
                return View(emailList);
            }
            catch (Exception ex)
            {
                // Log the error and show a friendly message
                ViewData["ErrorMessage"] = "Unable to load emails. Please check your connection.";
                return View(new List<Models.Email>());
            }
        }

        // GET: Email/Send
        public IActionResult Send()
        {
            try
            {
                var userRole = GetCurrentUserRole();
                if (userRole == EnumHelper.UserRole.User)
                    return Forbid();

                ViewData["Users"] = _context.Users
                    .Select(u => new User { Id = u.Id, Name = u.Name }) // light projection
                    .ToList();

                return View(new SendEmailViewModel());
            }
            catch
            {
                ViewData["ErrorMessage"] = "Unable to load users. Please check your connection.";
                ViewData["Users"] = new List<User>();
                return View(new SendEmailViewModel());
            }
        }

        // POST: Email/Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(SendEmailViewModel model)
        {
            try
            {
                var userRole = GetCurrentUserRole();
                if (userRole == EnumHelper.UserRole.User)
                    return Forbid();

                if (ModelState.IsValid)
                {
                    var email = new Models.Email
                    {
                        Sender = GetCurrentUserId(),
                        Receiver = model.ReceiverId,
                        Message = model.Message,
                        CreatedAt = DateTime.Now
                    };

                    _context.Emails.Add(email);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Email sent successfully!";
                    return RedirectToAction(nameof(Index));
                }

                // repopulate users when returning the form
                ViewData["Users"] = _context.Users
                    .Select(u => new User { Id = u.Id, Name = u.Name })
                    .ToList();

                return View(model);
            }
            catch
            {
                ViewData["ErrorMessage"] = "Unable to send email. Please check your connection.";
                ViewData["Users"] = _context.Users
                    .Select(u => new User { Id = u.Id, Name = u.Name })
                    .ToList();
                return View(model);
            }
        }
        // GET: Email/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var email = await _context.Emails
                    .Include(e => e.SenderNavigation)
                    .Include(e => e.ReceiverNavigation)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (email == null)
                {
                    return NotFound();
                }

                var currentUserId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                // Check if user has permission to view this email
                if (userRole != EnumHelper.UserRole.Admin &&
                    email.Sender != currentUserId &&
                    email.Receiver != currentUserId)
                {
                    return Forbid();
                }

                return View(email);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Unable to load email details.";
                return RedirectToAction(nameof(Index));
            }
        }

        private long GetCurrentUserId()
        {
            // Use session-based authentication like in AccountController
            var userId = HttpContext.Session.GetInt32("UserId");
            return userId ?? 0;
        }

        private EnumHelper.UserRole GetCurrentUserRole()
        {
            // Use session-based role like in AccountController
            var roleString = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(roleString))
                return EnumHelper.UserRole.User;

            // Map role names to enum values
            return roleString.ToLower() switch
            {
                "admin" => EnumHelper.UserRole.Admin,
                "nickname" => EnumHelper.UserRole.Nickname,
                _ => EnumHelper.UserRole.User
            };
        }
    }

    public class SendEmailViewModel
    {
        public long ReceiverId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

