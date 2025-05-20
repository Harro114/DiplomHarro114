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
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger;

    public ProfileController(ApplicationDbContext context, ILogger<ProfileController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }
        
            var user = await _context.Accounts.FirstOrDefaultAsync(a =>
                int.Parse(userId) == a.Id); 
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }
        
            var userProfileDto = new UserProfileDto
            {
                Id = user.Id,
                Name = user.Username,
                LastName = user.UserLastName,
                FirstName = user.UserFirstName,
                isBlocked = user.IsBlocked ?? false,
            };

            return Ok(userProfileDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось получить профиль");
            return BadRequest("Не удалось получить профиль");
            throw;
        }
    }


    [HttpGet("expHistory")]
    public async Task<ActionResult<ExpHistoryUserDTO>> GetExphistoryUser()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }

            var historyDto = new ExpHistoryUserDTO
            {
                Data = new List<DataExpHistoryDTO>()
            };

            var history = await _context.ExpChanges.Where(e => e.AccountId == int.Parse(userId))
                .OrderByDescending(e => e.CreatedAt).ToListAsync();
            foreach (var his in history)
            {
                historyDto.Data.Add(new DataExpHistoryDTO
                {
                    CreatedAt = his.CreatedAt,
                    Value = his.Value,
                    Discription = his.Discription,
                });
            }

            return Ok(historyDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось получить историю изменения баланса");
            return BadRequest("Не удалось получить историю изменения баланса");
            throw;
        }
    }

    [HttpGet("userDiscounts")]
    public async Task<ActionResult<getUserDiscountsDTO>> UserDiscounts()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }

            var noneActivatedDiscount = await _context.UserDiscounts.Where(ud => ud.AccountId == int.Parse(userId))
                .Join(_context.Discounts.Where(d => d.isArchived == false), ud => ud.DiscountId, d => d.Id, (ud, d) => new
                {
                    Id = ud.Id,
                    Name = d.Name,
                    Description = d.Description,
                    DiscountSize = d.DiscountSize, 
                })
                .ToListAsync();
            var activatedDiscount = await _context.UserDiscountsActivated.Where(ud => ud.AccountId == int.Parse(userId))
                .Join(_context.Discounts, ud => ud.DiscountId, d => d.Id, (ud, d) => new
                {
                    Id = ud.Id,
                    Name = d.Name,
                    Description = d.Description,
                    DiscountSize = d.DiscountSize
                }).ToListAsync();
            if (!noneActivatedDiscount.Any() && !activatedDiscount.Any())
            {
                return NotFound("Скидки для данного пользователя не найдены.");
            }

            var result = new getUserDiscountsDTO
            {
                Discounts = new List<usrDiscountsDTO>()
            };
            foreach (var nad in noneActivatedDiscount)
            {
                result.Discounts.Add(new usrDiscountsDTO
                {
                    Id = nad.Id,
                    Name = nad.Name,
                    Description = nad.Description ?? "",
                    DiscountSize = nad.DiscountSize,
                    isActivated = false
                });
            }

            foreach (var ad in activatedDiscount)
            {
                result.Discounts.Add(new usrDiscountsDTO
                {
                    Id = ad.Id,
                    Name = ad.Name,
                    Description = ad.Description ?? "",
                    DiscountSize = ad.DiscountSize,
                    isActivated = true
                });
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось получить скидки пользователя");
            return BadRequest("Не удалось получить скидки пользователя");
        }
    }


    [HttpGet("checkRole")]
    [Authorize]
    public async Task<ActionResult<AccountRole>> CheckRole()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }
            var role = await _context.AccountRole.FirstOrDefaultAsync(r => r.AccountId == int.Parse(userId));
            return Ok(role);
            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось вернуть роль");
            return BadRequest("Не удалось вернуть роль");
            throw;
        }
    }

    [HttpGet("getExpCount")]
    [Authorize]
    public async Task<ActionResult<int>> GetExpCount()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }
            var wallet = await _context.ExpUsersWallets.FirstOrDefaultAsync(euw => euw.AccountId == int.Parse(userId));
            if (wallet == null)
            {
                return BadRequest("Кошелек не найден");
            }
            return Ok(wallet.ExpValue);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось вернуть баланс пользователя");
            return BadRequest("Не удалось вернуть баланс пользователя");
            throw;
        }
    }
    
}