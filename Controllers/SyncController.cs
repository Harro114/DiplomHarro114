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
            var activatingDiscounts = _context.UserDiscountsActivated
                .Join(_context.Discounts, uda => uda.DiscountId, d => d.Id, (uda, d) => new { uda, d })
                .ToList();
            foreach (var discount in activatingDiscounts)
            {
                await _context.UserDiscountsActivatedHistory.AddAsync(new UserDiscountsActivatedHistory
                {
                    Id = discount.uda.Id,
                    AccountId = discount.uda.AccountId,
                    DiscountId = discount.uda.DiscountId,
                    DateActivateDiscount = discount.uda.DateActivateDiscount,
                    ProductsId = discount.d.ProductsId,
                    CategoriesId = discount.d.CategoriesId,
                    DiscountName = discount.d.Name
                    
                });
            }

            _context.UserDiscountsActivated.RemoveRange(activatingDiscounts.Select(x => x.uda).ToList());
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