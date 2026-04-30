using System.Diagnostics;
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BeFit.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Szybkie przełączanie między kontami użytkowników
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        // Import przykładowych danych 
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public HomeController(
            ILogger<HomeController> logger,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _environment = environment;
        }


        [HttpPost]
        public async Task<IActionResult> SwitchUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index");
        }

        // import danych z pliku JSON
        [HttpPost]
        public async Task<IActionResult> ImportData(IFormFile file)
        {
            string filePath = Path.Combine(_environment.ContentRootPath, "sessions.json");

            if (System.IO.File.Exists(filePath))
            {
                var json = await System.IO.File.ReadAllTextAsync(filePath);
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var sessions = System.Text.Json.JsonSerializer.Deserialize<List<Session>>(json, options);
                if (sessions != null )
                {

                    var currentUserId = _userManager.GetUserId(User);

                    foreach (var session in sessions)
                    {
                        session.Id = 0; 
                        session.UserId = currentUserId;
                    }

                    _context.Session.AddRange(sessions);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        // Czyszczenie rekordów sesji treningowych
        [HttpPost]
        public async Task<IActionResult> ClearSessions()
        {
            _context.Session.RemoveRange(_context.Session);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
