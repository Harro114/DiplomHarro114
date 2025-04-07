using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Diplom.Data;
using Diplom.Syncing;
using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

// Подключение к базе данных
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Настройка HttpClient
builder.Services.AddHttpClient();

// Настройка Hangfire с PostgreSQL хранилищем
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("HangfirePg"))
);

// Регистрация нужных сервисов
builder.Services.AddTransient<SynchronizationOrders>();

// Добавление Hangfire сервера для обработки задач
builder.Services.AddHangfireServer();

builder.Services.AddRazorPages();
builder.Services.AddControllers();

var app = builder.Build();

// Настройка Hangfire до выполнения заданий
using (var scope = app.Services.CreateScope())
{
    var synchronizationOrders = scope.ServiceProvider.GetRequiredService<SynchronizationOrders>();
    synchronizationOrders.ConfigureJobs();
}

// Если не в режиме разработки, настраиваем обработку ошибок
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();
app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();

// Подключение панели управления Hangfire
app.UseHangfireDashboard("/hangfire");

app.Run();