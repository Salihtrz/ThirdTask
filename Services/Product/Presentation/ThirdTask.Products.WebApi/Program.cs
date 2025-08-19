using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using ThirdTask.Jwt.Tools;
using ThirdTask.Products.Application.Features.CQRS.Handlers.ProductHandlers;
using ThirdTask.Products.Application.GlobalExceptions.Exceptions;
using ThirdTask.Products.Application.Interfaces;
using ThirdTask.Products.Application.RabbitMQ;
using ThirdTask.Products.Application.Services;
using ThirdTask.Products.Persistence.Context;
using ThirdTask.Products.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<productContext>(options =>
{
    options.UseNpgsql(connectionString);
});

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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Reader", policy => policy.RequireRole("Reader"));
    options.AddPolicy("Writer", policy => policy.RequireRole("Writer"));
    options.AddPolicy("ReaderOrWriter", policy => policy.RequireRole("Reader", "Writer"));
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("ProductRateLimit", c =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: "global",
            factory: f => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 8,
                Window = TimeSpan.FromSeconds(10),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            })
    );
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<GetProductByIdQueryHandler>();
builder.Services.AddScoped<GetProductQueryHandler>();
builder.Services.AddScoped<CreateProductCommandHandler>();
builder.Services.AddScoped<UpdateProductCommandHandler>();
builder.Services.AddScoped<RemoveProductCommandHandler>();
builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
builder.Services.AddHttpClient<ILogService, LogService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6381";
});

var app = builder.Build();

app.UseRateLimiter();
app.UseMiddleware<ErrorHandlerMiddleware>();

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
