using Microsoft.EntityFrameworkCore;
using API_website.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.CookiePolicy;
using API_website.DataAccess.Postgres.Repositories;
using System.Net;
using API_website.Infrastructure;
using API_website.DataAccess.Postgres.Entities;
using API_website.Application.Interfaces.Auth;

var builder = WebApplication.CreateSlimBuilder(args);

// Настройка CORS для определенных источников (например, для production)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins("https://your-allowed-origin.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

// Настройка логирования
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Добавление контроллеров
builder.Services.AddControllers();

// Регистрация сервиса для работы с видео и пользователями
builder.Services.AddScoped<VideoService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserEntities>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<JwpProvider>();

// Регистрация UserRepository как реализацию IUserService
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwpProvider>();

// Настройка базы данных через DbContext
builder.Services.AddDbContext<DBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Настройка Kestrel для использования определенного IP-адреса и порта
builder.WebHost.UseKestrel(options =>
{
    options.Listen(IPAddress.Loopback, 8080); // Установите IP и порт
});

// Авто мапер регестрация
builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();
    dbContext.Database.Migrate(); // Применяет миграции и создает базу данных
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();

// Глобальная обработка ошибок
app.UseExceptionHandler("/error");

// Поддержка CORS для production
app.UseCors("AllowSpecificOrigins");

// Маршрутизация запросов на контроллеры
app.MapControllers();

app.Run();
