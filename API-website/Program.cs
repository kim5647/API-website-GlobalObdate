using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json.Serialization;

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

// ��������� �����������
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ���������� ������������
builder.Services.AddControllers();

// ����������� ������� ��� ������ � ����� � ��������������
builder.Services.AddScoped<VideoService>();
builder.Services.AddScoped<UserService>();

// ����������� UserRepository ��� ���������� IUserService
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ��������� ���� ������ ����� DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� Kestrel ��� ������������� ������������� IP-������ � �����
builder.WebHost.UseKestrel(options =>
{
    options.Listen(IPAddress.Loopback, 8080); // ���������� IP � ����
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // ��������� �������� � ������� ���� ������
}

// ���������� ��������� ������
app.UseExceptionHandler("/error");

// ��������� CORS ��� production
app.UseCors("AllowSpecificOrigins");

// ������������� �������� �� �����������
app.MapControllers();

app.Run();