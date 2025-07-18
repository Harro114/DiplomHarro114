﻿using Gamification.Data;
using Gamification.Models;
using Gamification.Models.DTO;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Gamification.Syncing;

public class SynchronizationUsers
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<SynchronizationOrders> _logger;
    private readonly IRecurringJobManager _recurringJobManager;

    public SynchronizationUsers(ApplicationDbContext context, IHttpClientFactory httpClientFactory,
        IConfiguration config, ILogger<SynchronizationOrders> logger, IRecurringJobManager recurringJobManager)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
        _recurringJobManager = recurringJobManager;
    }

    public void ConfigureJobs()
    {
        _recurringJobManager.AddOrUpdate<SynchronizationUsers>(
            "SyncUsers",
            x => x.SyncUsers(),
            _config["SyncUsers:cron"],
            TimeZoneInfo.Local
        );
    }

    public async Task SyncUsers()
    {
        try
        {
            _logger.LogInformation("Запущена синхронизация пользователей {Time}", DateTime.UtcNow);

            var baseAddress = _config["HttpClient:magazin_api"];
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseAddress);

            var response = await client.GetAsync("/sync_users");

            var usersResponse = await response.Content.ReadFromJsonAsync<UsersResponseDTO>();

            var users = usersResponse?.user;

            _logger.LogInformation("Количество новых пользователей {Count}", users?.Count);

            if (users is null || !users.Any()) return;

            // Список существующих пользователей (кэш)
            var existingAccounts = await _context.Accounts.AsNoTracking().ToListAsync();

            foreach (var user in users)
            {
                // Проверяем наличие пользователя по Username
                if (existingAccounts.Any(u => u.Username == user.username))
                {
                    try
                    {
                        var existingAccount =
                            await _context.Accounts.FirstOrDefaultAsync(a => a.Username == user.username);
                        existingAccount.UserFirstName = user.first_name;
                        existingAccount.UserLastName = user.last_name;
                        existingAccount.Username = user.username;
                        existingAccount.Sex = user.sex;
                        existingAccount.IsBlocked = user.is_blocked;
                        var accountPassword =
                            await _context.AccountPasswords.FirstOrDefaultAsync(a => a.AccountId == existingAccount.Id);
                        accountPassword.PasswordHash = user.password;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Не удалось обновить информацию по пользователю {user.username}");
                    }

                    await _context.SaveChangesAsync();
                    continue;
                }

                ;

                try
                {
                    await _context.Accounts.AddAsync(new Accounts
                    {
                        Username = user.username,
                        UserLastName = user.last_name,
                        UserFirstName = user.first_name,
                        Sex = user.sex,
                        IsBlocked = user.is_blocked,
                        CreatedAt = DateTime.SpecifyKind(user.created_at, DateTimeKind.Utc)
                    });

                    await _context.SaveChangesAsync();

                    var createdAccount = await _context.Accounts.AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Username == user.username);

                    if (createdAccount != null)
                    {
                        await _context.AccountPasswords.AddAsync(new AccountPasswords
                        {
                            AccountId = createdAccount.Id,
                            PasswordHash = user.password
                        });
                        await _context.ExpUsersWallets.AddAsync(new ExpUsersWallets
                        {
                            AccountId = createdAccount.Id,
                            ExpValue = 0
                        });
                        await _context.AccountRole.AddAsync(new AccountRole
                        {
                            AccountId = createdAccount.Id,
                            RoleId = 1
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Не удалось просинхирить пользователя {user.username}");
                }
            }
            _logger.LogInformation("Синхронизация пользователей успешно завершена");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Возникла ошибка синхронизации пользователей");
            throw;
        }
    }
}