using Gamification.Data;
using Gamification.Models;
using Gamification.Models.DTO;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Gamification.Syncing;

public class SynchronizationOrders
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<SynchronizationOrders> _logger;
    private readonly IRecurringJobManager _recurringJobManager;

    public SynchronizationOrders(ApplicationDbContext context, IHttpClientFactory httpClientFactory,
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

        _recurringJobManager.AddOrUpdate(
                "sync-orders-job", 
                () => SyncOrders(), 
                _config["SyncOrders:cron"], 
                TimeZoneInfo.Local); 

        _recurringJobManager.AddOrUpdate(
            "calculate-exp-job", 
            () => CalculateExp(), 
            _config["SyncOrders:cron"], 
            TimeZoneInfo.Local); 
    }


    public async Task SyncOrders()
    {
        try
        {
            _logger.LogInformation("Запущена синхронизация товаров в {Time}", DateTime.UtcNow);
            var lastDate = await _context.Config.FirstOrDefaultAsync(c => c.Name == "LastDateOrder");
            
            var lastDateOrder = lastDate?.ValueDate ?? new DateTime(2000, 4, 7);
            var baseAddress = _config["HttpClient:magazin_api"];
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseAddress);
            
            var response = await client.GetAsync($"/sync_orders?last_sync_date={lastDateOrder.ToString("yyyy-MM-dd HH:mm:ss")}");

            var ordersResponse = await response.Content.ReadFromJsonAsync<OrdersResponseDTO>();
            var orders = ordersResponse?.order;  

            _logger.LogInformation("Количество заказов: {Count}", orders?.Count);
            if (orders is null || !orders.Any()) return;

            foreach (var order in orders)
            {

                _context.Orders.Add(new Orders
                {
                    AccountId = order.account_id,
                    Amounts = order.amount,
                    DateLastOrder = order.order_date.ToUniversalTime()
                    
                });
            }
            
            await _context.SaveChangesAsync();
            var newLastDate = await _context.Orders.MaxAsync(o => o.DateLastOrder);
            var updateConfig = await _context.Config.SingleOrDefaultAsync(a => a.Name == "LastDateOrder");
            if (updateConfig != null) updateConfig.ValueDate = newLastDate;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Синхронизация заказов завершилась успешно.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Возникла ошибка синхронизации товаров");
            throw;
        }
    }

    public async Task CalculateExp()
    {
        try
        {
            _logger.LogInformation("Запушена синхронизация баланса пользователей {Time}", DateTime.UtcNow);
            var curToExp = await _context.Config.FirstOrDefaultAsync(c => c.Name == "rublesToExp");
            var currencyToExp = curToExp?.ValueFloat ?? 1f;

            var syncedOrders = await _context.Orders.ToListAsync();
            foreach (var order in syncedOrders)
            {
                var exp = (int)Math.Round(order.Amounts * currencyToExp);
                var wallet = await _context.ExpUsersWallets.FirstOrDefaultAsync(a => a.AccountId == order.AccountId);
                var balance = 0;
                if (wallet != null)
                {
                    balance = wallet.ExpValue + exp;
                    wallet.ExpValue += exp;
                }


                _context.ExpChanges.Add(new ExpChanges
                {
                    AccountId = order.AccountId,
                    ExpUserId = wallet.Id,
                    Value = exp,
                    CurrentBalance = balance,
                    CreatedAt = order.DateLastOrder,
                    Discription = "Начисление Exp за покупки в магазине"
                });
            }

            var deletOrders = _context.Orders;
            _context.RemoveRange(deletOrders);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Синхронизация расчета баланса пользователей завершилась успешно.");
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка расчета баланса пользователей");
            throw;
        }
    }
}