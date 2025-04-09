using Diplom.Data;
using Diplom.Models;
using Diplom.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExpController> _logger;

    public DiscountsController(ApplicationDbContext context, ILogger<ExpController> logger)
    {
        _logger = logger;

        try
        {
            _context = context;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing ExpController.");
            throw;
        }
    }

    [HttpPost("buyPrimaryDiscount")]
    public async Task<ActionResult<UserDiscounts>> BuyPrimaryDiscount(
        [FromBody] BuyPrimaryDiscountDTO buyPrimaryDiscountDto)
    {
        try
        {
            var discount = await _context.Discounts.FirstOrDefaultAsync(d => d.Id == buyPrimaryDiscountDto.DiscountId);
            var userExp =
                await _context.ExpUsersWallets.FirstOrDefaultAsync(a => a.AccountId == buyPrimaryDiscountDto.AccountId);
            if (discount == null || userExp == null)
            {
                return BadRequest("Not found AccountWallet or Discounts");
            }

            if (discount.isActive == false)
            {
                return BadRequest("Discount is not active");
            }

            if (discount.isPrimary == false)
            {
                return BadRequest("Discount is not primary");
            }

            if (discount.Amount <= userExp.ExpValue)
            {
                var newDiscountUser = new UserDiscounts
                {
                    AccountId = buyPrimaryDiscountDto.AccountId,
                    DiscountId = buyPrimaryDiscountDto.DiscountId
                };
                await _context.UserDiscounts.AddAsync(newDiscountUser);
                userExp.ExpValue = userExp.ExpValue - discount.Amount;
                var expChange = new ExpChanges
                {
                    AccountId = buyPrimaryDiscountDto.AccountId,
                    ExpUserId = userExp.Id,
                    Value = discount.Amount * -1,
                    CurrentBalance = userExp.ExpValue - discount.Amount,
                    Discription = $"Покупка скидки {discount.Name}"
                };
                await _context.ExpChanges.AddAsync(expChange);
                
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(BuyPrimaryDiscount), new { newDiscountUser },
                    new { newDiscountUser });
            }

            return BadRequest("Error buying primary discount");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while buying primary discount");
            return BadRequest("Error buying primary discount");
        }
    }

    [HttpPost("CombiningDiscounts")]
    public async Task<ActionResult<UserDiscounts>> CombiningDiscounts(
        [FromBody] CombiningDiscountsDTO combiningDiscountsDto)
    {
        try
        {
            var userExp =
                await _context.ExpUsersWallets.FirstOrDefaultAsync(a => a.AccountId == combiningDiscountsDto.AccountId);
            if (userExp == null)
            {
                return BadRequest("Not found AccountWallet");
            }

            var discountExchange = await _context.ExchangeDiscounts.FirstOrDefaultAsync(ed =>
                ed.DiscountExchangeOneId == combiningDiscountsDto.DiscountOneId
                && ed.DiscountExchangeTwoId == combiningDiscountsDto.DiscountTwoId);
            if (discountExchange != null)
            {
                var discount = await _context.Discounts.FirstOrDefaultAsync(d => d.Id == discountExchange.DiscountId);
                var oneDiscount = await _context.UserDiscounts.FirstOrDefaultAsync(d => d.DiscountId == combiningDiscountsDto.DiscountOneId && d.AccountId == combiningDiscountsDto.AccountId);
                var twoDiscount = await _context.UserDiscounts.FirstOrDefaultAsync(d => d.DiscountId == combiningDiscountsDto.DiscountTwoId && d.AccountId == combiningDiscountsDto.AccountId);
                if (oneDiscount == null || twoDiscount == null)
                {
                    return BadRequest("Not found Discount");
                }
                if (discount.Amount <= userExp.ExpValue)
                {
                    var newDiscountUser = new UserDiscounts
                    {
                        AccountId = combiningDiscountsDto.AccountId,
                        DiscountId = discountExchange.DiscountId
                    };
                    await _context.UserDiscounts.AddAsync(newDiscountUser);
                    userExp.ExpValue = userExp.ExpValue - discount.Amount;
                    var expChange = new ExpChanges
                    {
                        AccountId = combiningDiscountsDto.AccountId,
                        ExpUserId = userExp.Id,
                        Value = discount.Amount * -1,
                        CurrentBalance = userExp.ExpValue - discount.Amount,
                        Discription = $"Объединение скидок до {discount.Name}"
                    };
                    await _context.ExpChanges.AddAsync(expChange);
                    var oneDiscountHistory = new UserDiscountsHistory
                    {
                        AccountId = oneDiscount.AccountId,
                        DiscountId = oneDiscount.DiscountId,
                        DateAccruals = oneDiscount.DateAccruals
                    };
                    var twoDiscountHistory = new UserDiscountsHistory
                    {
                        AccountId = twoDiscount.AccountId,
                        DiscountId = twoDiscount.DiscountId,
                        DateAccruals = twoDiscount.DateAccruals
                    };
                    await _context.UserDiscountsHistory.AddAsync(oneDiscountHistory);
                    await _context.UserDiscountsHistory.AddAsync(twoDiscountHistory);
                    _context.UserDiscounts.Remove(oneDiscount);
                    _context.UserDiscounts.Remove(twoDiscount);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction(nameof(CombiningDiscounts), new { newDiscountUser },
                        new { newDiscountUser });
                }
                else
                {
                    return BadRequest("Insufficient balance");
                }
                
            }

            return BadRequest("An error occurred while combining discounts");
        }
        catch (Exception e)
        {
            return BadRequest("An error occurred while combining discounts");
        }
    }

    [HttpPost("ActivatedDiscount")]
    public async Task<ActionResult<UserDiscountsActivated>> ActivatedDiscount(
        [FromBody] ActivatedDiscountDTO activatedDiscountDto)
    {
        try
        {
            var usrDiscount = await _context.UserDiscounts.FirstOrDefaultAsync(d =>
                d.DiscountId == activatedDiscountDto.DiscountId && d.AccountId == activatedDiscountDto.AccountId);
            if (usrDiscount != null)
            {
                var usrDiscountActivated = new UserDiscountsActivated
                {
                    AccountId = usrDiscount.AccountId,
                    DiscountId = usrDiscount.DiscountId
                };
                var DiscountHistory = new UserDiscountsHistory
                {
                    AccountId = usrDiscount.AccountId,
                    DiscountId = usrDiscount.DiscountId,
                    DateAccruals = usrDiscount.DateAccruals
                };
                await _context.UserDiscountsHistory.AddAsync(DiscountHistory);
                _context.UserDiscounts.Remove(usrDiscount);
                await _context.UserDiscountsActivated.AddAsync(usrDiscountActivated);
            }
            return BadRequest("An error occurred while activated discount");
            
        }
        catch (Exception e)
        {
            return BadRequest("An error occurred while activated discount");
        }
    }
}