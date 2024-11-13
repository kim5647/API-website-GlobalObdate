using Microsoft.EntityFrameworkCore;
using API_website.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.CookiePolicy;
using API_website.DataAccess.Postgres.Mapper.UserProfile;
using API_website.DataAccess.Postgres.Repositories;
using System.Net;
using API_website.Infrastructure;
using API_website.DataAccess.Postgres.Entities;
using API_website.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
using API_website.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

// Настройка CORS для определенных источников (например, для production)
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAllOrigins", builder =>
//    {
//        builder.WithOrigins("https://26.234.86.94") // Укажите IP-адрес сервера
//               .AllowAnyMethod()
//               .AllowAnyHeader()
//               .AllowCredentials();
//    });
//});


// Увеличение максимального размера тела запроса
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 500 * 1024 * 1024; // 500 MB
    options.Limits.MaxRequestBufferSize = 500 * 1024 * 1024; // 500 MB
});

// Увеличение лимита для multipart-запросов
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 500 * 1024 * 1024; // 500 MB
});

builder.Services.AddApiAuthentication(builder.Configuration);

// Настройка логирования
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Добавление контроллеров
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

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
    options.Listen(IPAddress.Parse("26.234.86.94"), 8080, ListenOptions =>
    {
        //ListenOptions.UseHttps("", "");
    }); // Установите IP и порт
});

// Авто мапер регистрация
builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();
    dbContext.Database.Migrate(); // Применяет миграции и создает базу данных
}

//app.UseCookiePolicy(new CookiePolicyOptions
//{
//    MinimumSameSitePolicy = SameSiteMode.None, // Для кросс-доменной передачи Cookies
//    HttpOnly = HttpOnlyPolicy.Always,
//    Secure = CookieSecurePolicy.Always // Обязательно для SameSite=None
//});

// Поддержка CORS для production
//app.UseCors("AllowAllOrigins");
//app.UseAuthentication();
//app.UseAuthorization();

// добавляет страницу отладчика, показывающую детали ошибки
app.UseDeveloperExceptionPage();


// Глобальная обработка ошибок
//app.UseExceptionHandler("/error");

// Маршрутизация запросов на контроллеры
app.MapControllers();

app.Run();
