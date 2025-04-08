using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuYolu.Data;
using SuYolu.Models;

namespace SuYolu.Controllers
{
    [Authorize]
    public class WaterUsageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public WaterUsageController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GenerateConsumptions()
        {
            try
            {

                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return BadRequest("Kullanıcı bulunamadı.");
                }
                // Son 30 gün için rastgele su tüketim verileri oluştur
                var startDate = DateTime.UtcNow.AddDays(-30); // Son 30 gün
                var endDate = DateTime.Now;
                var random = new Random();

                // Tarih aralığını gün bazında döngüye al
                var currentDate = startDate;
                while (currentDate <= endDate)
                {
                    // Rastgele saat ekle
                    var randomHour = random.Next(0, 24);
                    var randomDate = currentDate.AddHours(randomHour).AddMinutes(random.Next(0, 60));
       

                    // Rastgele miktar (5 ile 50 litre arasında)
                    var randomAmount = 5 + (random.NextDouble() * 45);

                    // Yeni WaterConsumption nesnesi oluştur
                    var waterConsumption = new WaterConsumption
                    {
                        UserId = userId,
                        Date = randomDate,
                        AmountInLiters = Math.Round(randomAmount, 2)
                    };

                    // Veritabanına ekle
                    _context.WaterConsumptions.Add(waterConsumption);

                    // Bir sonraki güne geç
                    currentDate = currentDate.AddDays(1);
                }

                // Tüm değişiklikleri kaydet
                await _context.SaveChangesAsync();

                return Ok("Rastgele su tüketim verileri başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hata oluştu: {ex.Message}");
            }
        }
    }
}