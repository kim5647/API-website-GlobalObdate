using Microsoft.EntityFrameworkCore;
using API_website.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.CookiePolicy;
using API_website.DataAccess.Postgres.Mapper.UserProfile;
using API_website.DataAccess.Postgres.Repositories;
using AutoMapper;
using System.Net;
using API_website.Infrastructure;
using API_website.DataAccess.Postgres.Entities;
using API_website.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
using API_website.Extensions;

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

// ���������� ������������� ������� ���� �������
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 500 * 1024 * 1024; // 500 MB
    options.Limits.MaxRequestBufferSize = 500 * 1024 * 1024; // 500 MB
});

// ���������� ������ ��� multipart-��������
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 500 * 1024 * 1024; // 500 MB
});

builder.Services.AddApiAuthentication(builder.Configuration);

// ��������� �����������
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ���������� ������������
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

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

// ��������� CORS ��� production
app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

// ���������� ��������� ������
app.UseExceptionHandler("/error");

// ������������� �������� �� �����������
app.MapControllers();

app.Run();
