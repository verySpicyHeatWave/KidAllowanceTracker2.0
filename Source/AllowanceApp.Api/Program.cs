using AllowanceApp.Api.Endpoints;
using AllowanceApp.Api.Services;
using AllowanceApp.Core.Services;
using AllowanceApp.Data.Actors;
using AllowanceApp.Data.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
var DbPath = System.IO.Path.Join(path, "accounts.db");

builder.Services.AddDbContext<AccountContext>(options =>
    options.UseSqlite($"Data Source={DbPath}"));

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

app.UseHttpsRedirection();
app.SetAccountCreateEndpoints();
app.SetAccountReadEndpoints();
app.SetAccountUpdateEndpoints();
app.SetAccountDeleteEndpoints();

app.Run();