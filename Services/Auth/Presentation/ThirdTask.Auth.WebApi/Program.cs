using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using ThirdTask.Auth.Application.Exceptions;
using ThirdTask.Auth.Application.Interfaces;
using ThirdTask.Auth.Application.RabbitMQ;
using ThirdTask.Auth.Application.Services;
using ThirdTask.Auth.Domain.Entities;
using ThirdTask.Auth.Persistence.Context;
using ThirdTask.Jwt.Interfaces;
using ThirdTask.Jwt.Tools;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<authContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.Configure<IdentityOptions>(options =>
{
options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789þÞçÇöÖüÜýÝðÐ-._@+";
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProductUpdateWriter", policy => policy.RequireRole("Writer"));
});

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidAudience = JwtTokenDefaults.ValidAudience,
        ValidIssuer = JwtTokenDefaults.ValidIssuer,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtTokenDefaults.Key)),
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddHttpClient<ILogService, LogService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<authContext>().AddDefaultTokenProviders();
builder.Services.AddHostedService<ProductCreatedEventConsumer>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleware>();

using var scope = app.Services.CreateScope();
var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();

await roleService.CreateRoles("Reader");
await roleService.CreateRoles("Writer");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
