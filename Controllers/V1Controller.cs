using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuYolu.Data;
using SuYolu.Models;

namespace SuYolu.Controllers;

[ApiController]
[Route("api/[controller]")]
public class V1Controller : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    public V1Controller(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpGet]
    public IEnumerable<WaterConsumption> Get()
    {
        return _context.WaterConsumptions.ToList();
    }

    [HttpPost("AddWaterConsumption")]  // Explicit route for POST method
    public async Task<IActionResult> AddWaterConsumption([FromBody] WaterUsage waterUsage)
    {
        if (waterUsage == null)
        {
            return BadRequest(new { status = "FAIL", error = "Geçersiz Giriş" });
        }
        
        try
        {
            var user = await _userManager.FindByIdAsync(waterUsage.UserId);
        if (user == null)
        {
            return BadRequest(new { status = "FAIL", error = "Kullanıcı Kodu Hatalı" });
        }
            var waterConsumption = new WaterConsumption
            {
                UserId = waterUsage.UserId,
                Date = DateTime.Now,
                AmountInLiters = waterUsage.AmountInLiters
            };
            
            await _context.WaterConsumptions.AddAsync(waterConsumption);
            await _context.SaveChangesAsync();
            
            return Ok(new { status = "OK" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { status = "FAIL", error = ex.Message });
        }
    }
}