using Microsoft.EntityFrameworkCore;
using API_website.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.CookiePolicy;
using API_website.DataAccess.Postgres.Repositories;
using System.Net;
using API_website.Infrastructure;
using API_website.DataAccess.Postgres.Entities;
using API_website.Application.Interfaces.Auth;

var builder = WebApplication.CreateSlimBuilder(args);

// ��������� CORS ��� ������������ ���������� (��������, ��� production)
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

// ��������� �����������
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ���������� ������������
builder.Services.AddControllers();

// ����������� ������� ��� ������ � ����� � ��������������
builder.Services.AddScoped<VideoService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserEntities>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<JwpProvider>();

// ����������� UserRepository ��� ���������� IUserService
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwpProvider>();

// ��������� ���� ������ ����� DbContext
builder.Services.AddDbContext<DBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� Kestrel ��� ������������� ������������� IP-������ � �����
builder.WebHost.UseKestrel(options =>
{
    options.Listen(IPAddress.Loopback, 8080); // ���������� IP � ����
});

// ���� ����� �����������
builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();
    dbContext.Database.Migrate(); // ��������� �������� � ������� ���� ������
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();

// ���������� ��������� ������
app.UseExceptionHandler("/error");

// ��������� CORS ��� production
app.UseCors("AllowSpecificOrigins");

// ������������� �������� �� �����������
app.MapControllers();

app.Run();
