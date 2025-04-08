using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuYolu.Data;
using SuYolu.Models;

namespace SuYolu.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [Route("Profile/Index/{days?}")]
        public async Task<IActionResult> Index(double days=7)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var todayConsumption = await _context.WaterConsumptions
                .Where(w => w.UserId == user.Id && w.Date.Date == DateTime.Now.Date)
                .Select(w => Math.Round(w.AmountInLiters, 2))
                .SumAsync();
            ViewBag.TodayConsumption = todayConsumption;
            var weeklyTotal = await _context.WaterConsumptions
                .Where(w => w.UserId == user.Id && w.Date > DateTime.Now.AddDays(-7))
                .Select(w => w.AmountInLiters)
                .SumAsync();
            ViewBag.WeeklyTotal = Math.Round(weeklyTotal, 2);
            var monthlyTotal = await _context.WaterConsumptions
                .Where(w => w.UserId == user.Id && w.Date > DateTime.Now.AddDays(-30))
                .Select(w => w.AmountInLiters)
                .SumAsync();
            ViewBag.MonthlyTotal = Math.Round(monthlyTotal, 2);
            var allTimeTotal = await _context.WaterConsumptions
                .Where(w => w.UserId == user.Id)
                .Select(w => w.AmountInLiters)
                .SumAsync();
            ViewBag.AllTimeTotal = Math.Round(allTimeTotal, 2);
            var lastWeek = DateTime.Now.AddDays(-days);
            var consumptions = await _context.WaterConsumptions
                .Where(w => w.UserId == user.Id && w.Date > lastWeek)
                .GroupBy(w => w.Date.Date)
                .Select(g => new { 
                    Date = g.Key,
                    Total = Math.Round(g.Sum(w => w.AmountInLiters), 2)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
            ViewBag.WeeklyConsumption = consumptions;
            ViewBag.Today = await _context.WaterConsumptions
                .Where(w => w.UserId == user.Id && w.Date> DateTime.Now.AddDays(-1))
                
                .Select(g => new { 
                    g.Date,
                    Total = Math.Round(g.AmountInLiters, 2)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

ViewBag.days = days;
            return View();
        }

        [HttpGet]
        public IActionResult AddConsumption()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.UserId=user.Id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddConsumption(WaterConsumption consumption)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                consumption.UserId = user.Id;
                consumption.Date = DateTime.Now;

                _context.WaterConsumptions.Add(consumption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(consumption);
        }
    }
}