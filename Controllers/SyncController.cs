using Gamification.Data;
using Gamification.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gamification.Controllers;

[ApiController]
[Route("sync")]
public class SyncController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExpController> _logger;

    public SyncController(ApplicationDbContext context, ILogger<ExpController> logger)
    {
        _logger = logger;

        try
        {
            _context = context;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не удалось инициализировать синхронизацию скидок");
            throw;
        }
    }

    [HttpGet("SyncDiscounts")]
    public async Task<ActionResult<UserDiscountsActivated>> SyncDiscounts()
    {
        try
        {
            var activatingDiscounts = _context.UserDiscountsActivated.ToList();
            foreach (var discount in activatingDiscounts)
            {
                await _context.UserDiscountsActivatedHistory.AddAsync(new UserDiscountsActivatedHistory
                {
                    Id = discount.Id,
                    AccountId = discount.AccountId,
                    DiscountId = discount.DiscountId,
                    DateActivateDiscount = discount.DateActivateDiscount
                });
            }

            _context.UserDiscountsActivated.RemoveRange(activatingDiscounts);
            await _context.SaveChangesAsync();
            return Ok(activatingDiscounts);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось синхронизировать скидки");
            return BadRequest("Не удалось синхронизировать скидки");
            throw;
        }
    }
}