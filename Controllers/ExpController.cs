using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Gamification.Data;
using Gamification.Models;
using Gamification.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Gamification.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExpController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExpController> _logger;

    public ExpController(ApplicationDbContext context, ILogger<ExpController> logger)
    {
        _logger = logger;

        try
        {
            _context = context;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не удалось инициализировать контроллер Exp.");
            throw;
        }
    }

    [HttpPost("createUserExpWallet")]
    public async Task<ActionResult<ExpUsersWallets>> CreateUserExpWallet(
        [FromBody] ExpUsersWalletsDTO expUsersWalletsDto)
    {
        try
        {
            var accountExists = await _context.Accounts.AnyAsync(a => a.Id == expUsersWalletsDto.AccountId);
            if (!accountExists)
            {
                return BadRequest("Данного пользователя нет в системе.");
            }

            var accountInWallets = await _context.ExpUsersWallets
                .Where(w => w.AccountId == expUsersWalletsDto.AccountId)
                .CountAsync();
            if (accountInWallets != 0)
            {
                return BadRequest("Кошелек данного пользователя уже создан");
            }


            expUsersWalletsDto.ExpValue = 0;
            var expUsersWallets = new ExpUsersWallets
            {
                AccountId = expUsersWalletsDto.AccountId,
                ExpValue = 0
            };
            await _context.ExpUsersWallets.AddAsync(expUsersWallets);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateUserExpWallet), new { expUsersWalletsDto.AccountId },
                expUsersWalletsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating user exp wallet.");
            return StatusCode(500, "An error occurred while creating user exp wallet.");
        }
    }
}