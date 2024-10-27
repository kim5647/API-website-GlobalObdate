using Microsoft.EntityFrameworkCore;
using API_website.Application.Interfaces.Repositories;
using API_website.DataAccess.Postgres.Repositories;
using System.Net;

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

// Настройка логирования
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Добавление контроллеров
builder.Services.AddControllers();

// Регистрация сервиса для работы с видео и пользователями
builder.Services.AddScoped<VideoService>();
builder.Services.AddScoped<UserService>();

// Регистрация UserRepository как реализацию IUserService
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();

// Настройка базы данных через DbContext
builder.Services.AddDbContext<DBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Настройка Kestrel для использования определенного IP-адреса и порта
builder.WebHost.UseKestrel(options =>
{
    options.Listen(IPAddress.Loopback, 8080); // Установите IP и порт
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();
    dbContext.Database.Migrate(); // Применяет миграции и создает базу данных
}

// Глобальная обработка ошибок
app.UseExceptionHandler("/error");

// Поддержка CORS для production
app.UseCors("AllowSpecificOrigins");

// Маршрутизация запросов на контроллеры
app.MapControllers();

app.Run();
