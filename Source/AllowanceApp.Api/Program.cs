using AllowanceApp.Api.Endpoints;
using AllowanceApp.Api.Services;
using AllowanceApp.Core.Services;
using AllowanceApp.Data.Actors;
using AllowanceApp.Data.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

string? dbPath = builder.Configuration["Database:Path"];

if (string.IsNullOrEmpty(dbPath))
{
    var folder = Environment.SpecialFolder.LocalApplicationData;
    var localPath = Environment.GetFolderPath(folder);
    dbPath = Path.Combine(localPath, "accounts.db");
}

// Ensure directory exists
var dbDir = Path.GetDirectoryName(dbPath);
if (dbDir is not null && !Directory.Exists(dbDir))
{
    Directory.CreateDirectory(dbDir);
}

var connectionString = $"Data Source={dbPath}";
builder.Services.AddDbContext<AccountContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddHostedService<WeeklyAllowanceService>();
builder.Services.AddScoped<IAccountServiceActor, AccountServiceActor>();
builder.Services.AddScoped<AccountService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (!app.Environment.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AccountContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.SetAccountCreateEndpoints();
app.SetAccountReadEndpoints();
app.SetAccountUpdateEndpoints();
app.SetAccountDeleteEndpoints();

app.Run();

public partial class Program { }

