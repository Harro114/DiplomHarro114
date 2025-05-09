using Diplom.Data;
using Diplom.Models;
using Diplom.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Diplom.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExpController> _logger;

    public AdminController(ApplicationDbContext context, ILogger<ExpController> logger)
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

    [HttpGet("getAllUsers")]
    [Authorize]
    public async Task<ActionResult<GetAllUsersDTO>> GetAllUsers()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }
            var roleUser = await _context.AccountRole.FirstOrDefaultAsync(r => r.AccountId == int.Parse(userId));
            if (roleUser.RoleId != 2 || roleUser.RoleId == null)
            {
                return Unauthorized("Пользователь не является администратором!");
            }

            var users = await _context.Accounts
                .Join(_context.AccountRole, a => a.Id, ar => ar.AccountId, (a, ar) => new { a, ar })
                .Join(_context.Role, ar => ar.ar.RoleId, r => r.Id, (ar, r) => new { ar, r })
                .Select(x => new GetAllUsersDTO
                {
                    Id = x.ar.a.Id,
                    Username = x.ar.a.Username,
                    UserLastName = x.ar.a.UserLastName,
                    UserFirstName = x.ar.a.UserFirstName,
                    Sex = x.ar.a.Sex,
                    CreatedAt = x.ar.a.CreatedAt,
                    IsBlocked = x.ar.a.IsBlocked ?? false,
                    RoleId = x.ar.ar.RoleId, 
                    RoleName = x.r.Name
                }).ToListAsync();
            return Ok(users);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while getting all users");
            return BadRequest("Непредвиденная ошибка" + e);
            throw;
        }
    }

    [HttpPost("blockedUser")]
    [Authorize]
    public async Task<ActionResult<Accounts>> BlockedUser(
        [FromBody] BlockedUserDTO blockedUserDto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }
            var roleUser = await _context.AccountRole.FirstOrDefaultAsync(r => r.AccountId == int.Parse(userId));
            if (roleUser.RoleId != 2 || roleUser.RoleId == null)
            {
                return Unauthorized("Пользователь не является администратором!");
            }

            var user = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == blockedUserDto.UserId);
            if (user == null)
            {
                return BadRequest("Пользователь не найден");
            }
            else if (user.IsBlocked == blockedUserDto.IsBlocked)
            {
                return BadRequest("У пользователя уже данный статус блокировки");
            }
            else
            {
                user.IsBlocked = blockedUserDto.IsBlocked;
                await _context.SaveChangesAsync();
                return Ok(user);
            }

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось заблокировать пользователя");
            return BadRequest("Не удалось заблокировать пользователя" + e);
            throw;
        }
    }

    [HttpGet("getUserHistory")]
    [Authorize]
    public async Task<ActionResult<ExpChanges>> GetUserHistory(
        [FromQuery] int accountId
    )
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }
            var roleUser = await _context.AccountRole.FirstOrDefaultAsync(r => r.AccountId == int.Parse(userId));
            if (roleUser.RoleId != 2 || roleUser.RoleId == null)
            {
                return Unauthorized("Пользователь не является администратором!");
            }

            var user = await _context.ExpChanges
                .Where(ec => ec.AccountId == accountId)
                .Select(ec => new ExpChanges
                {
                    Id = ec.Id,
                    AccountId = ec.AccountId,
                    ExpUserId = ec.ExpUserId,
                    Value = ec.Value,
                    CurrentBalance = ec.CurrentBalance,
                    CreatedAt = ec.CreatedAt,
                    Discription = ec.Discription
                })
                .ToListAsync();
            if (user == null)
            {
                return BadRequest("Информация по пользователю не найдена");
            }
            return Ok(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось заблокировать пользователя");
            return BadRequest("Возникла непредвиденная ошибка");
            throw;
        }
    }

    [HttpPost("changeRole")]
    [Authorize]
    public async Task<ActionResult<Accounts>> ChangeRole(
        [FromBody] ChangeRoleDTO changeRoleDto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }
            var roleUser = await _context.AccountRole.FirstOrDefaultAsync(r => r.AccountId == int.Parse(userId));
            if (roleUser.RoleId != 2 || roleUser.RoleId == null)
            {
                return Unauthorized("Пользователь не является администратором!");
            }
            
            var roleAc = await _context.AccountRole.FirstOrDefaultAsync(ar => ar.AccountId == changeRoleDto.AccountId);
            var role = await _context.Role.FirstOrDefaultAsync(r => r.Id == changeRoleDto.RoleId);
            if (roleAc == null)
            {
                return BadRequest("Данный пользователь не найден");
            }
            else if (role == null)
            {
                return BadRequest("Данной роли нет");
            }
            else if (roleAc.RoleId == changeRoleDto.RoleId)
            {
                return BadRequest("Данная роль уже выбрана");
            }
            else
            {
                roleAc.RoleId = changeRoleDto.RoleId;
                await _context.SaveChangesAsync();
                return Ok(roleAc);
            }
            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось изменит роль пользователя");
            return BadRequest("Не удалось изменить роль пользователя" + e);
            throw;
        }
    }

    [HttpGet("getRoles")]
    [Authorize]
    public async Task<ActionResult<Roles>> GetRoles()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }
            var roleUser = await _context.AccountRole.FirstOrDefaultAsync(r => r.AccountId == int.Parse(userId));
            if (roleUser.RoleId != 2 || roleUser.RoleId == null)
            {
                return Unauthorized("Пользователь не является администратором!");
            }
            
            var roles = await _context.Role.ToListAsync();
            return Ok(roles);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось получить роли");
            return BadRequest("Непредвиденная ошибка" + e);
            throw;
        }
    }

    [HttpGet("getAllProducts")]
    [Authorize]
    public async Task<ActionResult<ProductsStore>> GetAllProducts()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }
            var roleUser = await _context.AccountRole.FirstOrDefaultAsync(r => r.AccountId == int.Parse(userId));
            if (roleUser.RoleId != 2 || roleUser.RoleId == null)
            {
                return Unauthorized("Пользователь не является администратором!");
            }
            
            var products = await _context.ProductsStore.ToListAsync();
            return Ok(products);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось вернуть список товаров");
            return BadRequest("Не удалось вернуть список товаров" + e);
            throw;
        }
    }

    [HttpGet("getAllCategories")]
    [Authorize]
    public async Task<ActionResult<CategoriesStore>> GetAllCategories()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }
            var roleUser = await _context.AccountRole.FirstOrDefaultAsync(r => r.AccountId == int.Parse(userId));
            if (roleUser.RoleId != 2 || roleUser.RoleId == null)
            {
                return Unauthorized("Пользователь не является администратором!");
            }
            var categories = await _context.CategoriesStore.ToListAsync();
            return Ok(categories);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось вернуть список категорий");
            return BadRequest("Не удалось вернуть список категорий" + e);
            throw;
        }
    }
    
    [HttpPost("createDiscount")]
    [Authorize]
    public async Task<ActionResult<Discounts>> CreatedDiscount(
        [FromBody] Discounts discounts)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Токен недействителен или не содержит необходимую информацию.");
            }
            var roleUser = await _context.AccountRole.FirstOrDefaultAsync(r => r.AccountId == int.Parse(userId));
            if (roleUser.RoleId != 2 || roleUser.RoleId == null)
            {
                return Unauthorized("Пользователь не является администратором!");
            }
            
            var products = await _context.ProductsStore.Where(a => a.isActive == true).ToListAsync();
            var categories = await _context.CategoriesStore.Where(a => a.isActive == true).ToListAsync();
            foreach (var product in discounts.ProductsId)
            {
                if (!products.Any(p => p.Id == product.Id))
                {
                    return BadRequest("Не все товары активны");
                }
            }
            foreach (var category in discounts.CategoriesId)
            {
                if (!categories.Any(p => p.Id == category.Id))
                {
                    return BadRequest("Не все категории активны");
                }

            }


            _context.Discounts.Add(discounts);
            await _context.SaveChangesAsync();
            return Ok(discounts);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось создать скидку");
            return BadRequest("Не удалось создать скидку" + e);
            throw;
        }
    }
}