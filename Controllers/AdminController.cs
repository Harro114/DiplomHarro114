using Diplom.Data;

namespace Diplom.Controllers;

public class AdminController
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
    
    
}