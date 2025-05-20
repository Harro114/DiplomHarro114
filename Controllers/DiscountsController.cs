using System.Security.Claims;
using Gamification.Data;
using Gamification.Models;
using Gamification.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gamification.Controllers;

[Authorize]
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
            _logger.LogError(ex, "Не удалось инициализировать DiscountsController");
            throw;
        }
    }

    [HttpPost("buyPrimaryDiscount")]
    [Authorize]
    public async Task<ActionResult<UserDiscounts>> BuyPrimaryDiscount(
        [FromBody] BuyPrimaryDiscountDTO buyPrimaryDiscountDto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("Токен недействителен или не содержит необходимую информацию.");
            }

            var discount = await _context.Discounts.FirstOrDefaultAsync(d => d.Id == buyPrimaryDiscountDto.DiscountId && d.isArchived == false);
            var userExp =
                await _context.ExpUsersWallets.FirstOrDefaultAsync(a => a.AccountId == int.Parse(userId));
            if (discount == null || userExp == null)
            {
                return BadRequest("Не найден кошелек или скидка");
            }

            if (discount.isActive == false)
            {
                return BadRequest("Скидка не активна");
            }

            if (discount.isPrimary == false)
            {
                return BadRequest("Скидка не является первичной");
            }

            if (discount.EndDate < DateTime.UtcNow && discount.EndDate != null)
            {
                return BadRequest("Срок действия скидки уже закончился");
            }

            if (discount.StartDate > DateTime.UtcNow)
            {
                return BadRequest("Скидка еще не началась");
            }

            if (discount.Amount <= userExp.ExpValue)
            {
                var newDiscountUser = new UserDiscounts
                {
                    AccountId = int.Parse(userId),
                    DiscountId = buyPrimaryDiscountDto.DiscountId
                };
                await _context.UserDiscounts.AddAsync(newDiscountUser);
                userExp.ExpValue = userExp.ExpValue - discount.Amount;
                var expChange = new ExpChanges
                {
                    AccountId = int.Parse(userId),
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

            return BadRequest("Не удалось купить скидку");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось купить скидку");
            return BadRequest("Не удалось купить скидку");
        }
    }

    [HttpPost("CombiningDiscounts")]
    [Authorize]
    public async Task<ActionResult<UserDiscounts>> CombiningDiscounts(
        [FromBody] CombiningDiscountsDTO combiningDiscountsDto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }

            var userExp =
                await _context.ExpUsersWallets.FirstOrDefaultAsync(a => a.AccountId == int.Parse(userId));
            if (userExp == null)
            {
                return BadRequest("Не найден кошелек");
            }

            var discountExchange = await _context.ExchangeDiscounts.FirstOrDefaultAsync(ed =>
                (ed.DiscountExchangeOneId == combiningDiscountsDto.DiscountOneId
                 && ed.DiscountExchangeTwoId == combiningDiscountsDto.DiscountTwoId) ||
                (ed.DiscountExchangeOneId == combiningDiscountsDto.DiscountTwoId
                 && ed.DiscountExchangeTwoId == combiningDiscountsDto.DiscountOneId));
            if (discountExchange != null)
            {
                var discount = await _context.Discounts.FirstOrDefaultAsync(d => d.Id == discountExchange.DiscountId);
                var oneDiscount = await _context.UserDiscounts
                    .FirstOrDefaultAsync(d =>
                    d.DiscountId == combiningDiscountsDto.DiscountOneId && d.AccountId == int.Parse(userId));
                var twoDiscount = await _context.UserDiscounts.FirstOrDefaultAsync(d =>
                    d.DiscountId == combiningDiscountsDto.DiscountTwoId && d.AccountId == int.Parse(userId));
                if (oneDiscount == null || twoDiscount == null)
                {
                    return BadRequest("Не найдена скидка для объединения");
                }

                var oneDisc = await _context.Discounts.FirstOrDefaultAsync(z => z.Id == oneDiscount.DiscountId);
                var twoDisc = await _context.Discounts.FirstOrDefaultAsync(z => z.Id == twoDiscount.DiscountId);
                if (discount.isActive = false || discount.isArchived == true || oneDisc.isArchived == true || twoDisc.isArchived == true)
                {
                    return BadRequest("Скидка для объединения не активна");
                }

                if (discount.EndDate < DateTime.UtcNow && discount.EndDate != null)
                {
                    return BadRequest("Срок действия скидки закончился");
                }

                if (discount.StartDate > DateTime.UtcNow)
                {
                    return BadRequest("Скидка еще не активна");
                }
                

                if (discount.Amount <= userExp.ExpValue)
                {
                    var newDiscountUser = new UserDiscounts
                    {
                        AccountId = int.Parse(userId),
                        DiscountId = discountExchange.DiscountId
                    };
                    await _context.UserDiscounts.AddAsync(newDiscountUser);
                    userExp.ExpValue = userExp.ExpValue - discount.Amount;
                    var expChange = new ExpChanges
                    {
                        AccountId = int.Parse(userId),
                        ExpUserId = userExp.Id,
                        Value = discount.Amount * -1,
                        CurrentBalance = userExp.ExpValue - discount.Amount,
                        Discription = $"Объединение скидок до {discount.Name}"
                    };
                    await _context.ExpChanges.AddAsync(expChange);
                    var oneDiscountHistory = new UserDiscountsHistory
                    {
                        Id = oneDiscount.Id,
                        AccountId = oneDiscount.AccountId,
                        DiscountId = oneDiscount.DiscountId,
                        DateAccruals = oneDiscount.DateAccruals
                    };
                    var twoDiscountHistory = new UserDiscountsHistory
                    {
                        Id = twoDiscount.Id,
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
                    return BadRequest("Не достаточный баланс");
                }
            }

            return BadRequest("Не удалось объединить скидки");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось объединить скидки");
            return BadRequest("Не удалось объединить скидки");
        }
    }

    [HttpPost("ActivatedDiscount")]
    public async Task<ActionResult<UserDiscountsActivated>> ActivatedDiscount(
        [FromBody] ActivatedDiscountDTO activatedDiscountDto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }

            var usrDiscount = await _context.UserDiscounts.FirstOrDefaultAsync(d =>
                d.Id == activatedDiscountDto.Id && d.AccountId == int.Parse(userId));
            if (usrDiscount != null)
            {
                var usrDiscountActivated = new UserDiscountsActivated
                {
                    AccountId = usrDiscount.AccountId,
                    DiscountId = usrDiscount.DiscountId
                };
                var discountHistory = new UserDiscountsHistory
                {
                    Id = usrDiscount.Id,
                    AccountId = usrDiscount.AccountId,
                    DiscountId = usrDiscount.DiscountId,
                    DateAccruals = usrDiscount.DateAccruals
                };
                await _context.UserDiscountsActivated.AddAsync(usrDiscountActivated);
                await _context.UserDiscountsHistory.AddAsync(discountHistory);
                _context.UserDiscounts.Remove(usrDiscount);
                await _context.SaveChangesAsync();
                return Ok(usrDiscountActivated);
            }

            return BadRequest("Не удалось активировать скидку");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось активировать скидку");
            return BadRequest("Не удалось активировать скидку");
        }
    }

    [HttpGet("checkExchange/{discountId1}/{discountId2}")]
    public async Task<ActionResult<checkExchangeDTO>> CheckExchange(int discountId1, int discountId2)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }

            var discount = await _context.ExchangeDiscounts.FirstOrDefaultAsync(ed =>
                (ed.DiscountExchangeOneId == discountId1 && ed.DiscountExchangeTwoId == discountId2) ||
                (ed.DiscountExchangeOneId == discountId2 && ed.DiscountExchangeTwoId == discountId1));
            if (discount == null)
            {
                return Ok(new checkExchangeDTO
                {
                    hasDiscount = false
                });
            }

            var actualeDiscount = await _context.Discounts.FirstOrDefaultAsync(d => d.Id == discount.DiscountId);
            var oneDisc = await _context.Discounts.FirstOrDefaultAsync(z => z.Id == discount.DiscountExchangeOneId);
            var twoDisc = await _context.Discounts.FirstOrDefaultAsync(z => z.Id == discount.DiscountExchangeTwoId);
            if (actualeDiscount.isArchived == true || oneDisc.isArchived == true || twoDisc.isArchived == true)
            {
                return BadRequest("Скидка удалена");
            }
            return Ok(new checkExchangeDTO
            {
                hasDiscount = true,
                Discount = actualeDiscount
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось вернуть наличие объединения");
            return BadRequest("Не удалось вернуть наличие объединения");
            throw;
        }
    }

    [HttpGet("getPrimaryDiscount")]
    public async Task<ActionResult<GetPrimaryDiscountDTO>> GetPrimaryDiscount()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }
            
            var discounts = await _context.Discounts
                .Where(d => d.isPrimary == true && (d.EndDate > DateTime.UtcNow || d.EndDate == null) && d.isArchived == false)
                .Select(d =>
                    new Discounts
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Description = d.Description,
                        isActive = d.isActive,
                        DiscountSize = d.DiscountSize,
                        StartDate = d.StartDate,
                        EndDate = d.EndDate,
                        ProductsId = d.ProductsId,
                        CategoriesId = d.CategoriesId,
                        Amount = d.Amount,
                        isPrimary = d.isPrimary
                    })
                .ToListAsync();
            var getDto = discounts;
            return Ok(getDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось получить список первичных скидок");
            return BadRequest("Не удалось получить список первичных скидок");
            throw;
        }
    }

    [HttpGet("getAllDiscountsUser")]
    public async Task<ActionResult<GetAllDiscountsUserDTO>> GetAllDiscountsUser()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }

            var discountsNoneActivated = await _context.UserDiscounts
                .Where(ud => ud.AccountId == int.Parse(userId))
                .Join(_context.Discounts.Where(d => d.isArchived == false), ud => ud.DiscountId, d => d.Id, (ud, d) => new { ud, d })
                .Where(x =>
                    x.ud.AccountId == int.Parse(userId))
                .Select(x => new DiscountDTO
                {
                    Id = x.ud.Id,
                    DiscountId = x.d.Id,
                    Name = x.d.Name,
                    Description = x.d.Description,
                    isActive = x.d.isActive,
                    DiscountSize = x.d.DiscountSize,
                    StartDate = x.d.StartDate,
                    EndDate = x.d.EndDate,
                    ProductsId = x.d.ProductsId,
                    CategoriesId = x.d.CategoriesId,
                    Amount = x.d.Amount,
                    isPrimary = x.d.isPrimary
                })
                .ToListAsync();
            
            var getDto = new GetAllDiscountsUserDTO()
            {
                Discount = new List<DiscountDTO>()
            };
            
            foreach (var dis in discountsNoneActivated)
            {
                if ((dis.EndDate >= DateTime.UtcNow && dis.EndDate != null || dis.EndDate == null) && (dis.StartDate <= DateTime.UtcNow)) {
                    getDto.Discount.Add(dis);
                }
            }

           
            return Ok(getDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось получить список скидок пользователей");
            return BadRequest("Не удалось получить список скидок пользователей");
            throw;
        }
    }
}