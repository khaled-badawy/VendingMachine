using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Text;
using Vending_Machine.BL;
using Vending_Machine.DAL;

var builder = WebApplication.CreateBuilder(args);

#region Default

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

#region Database

builder.Services.AddDbContext<VendingMachineContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VendingMachineConncetionString")));

#endregion

#region Identity

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
    .AddEntityFrameworkStores<VendingMachineContext>()
    .AddDefaultTokenProviders();

#endregion

#region JWT Authentication

builder.Services.Configure<Jwt>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = false;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

#endregion

#region Authorization

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Buyers", policy =>
        policy
            .RequireClaim(ClaimTypes.Role, "buyer")
            .RequireClaim(ClaimTypes.NameIdentifier));

    options.AddPolicy("Sellers", policy =>
        policy
            .RequireClaim(ClaimTypes.Role, "seller")
            .RequireClaim(ClaimTypes.NameIdentifier));
});

#endregion

#region Repos

builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IBuyerProductRepo, BuyerProductRepo>();

#endregion

#region Manager

builder.Services.AddScoped<IProductManager, ProductManager>();
builder.Services.AddScoped<IUserManagementManager, UserManagementManager>();
builder.Services.AddScoped<IBuyerProductManager, BuyerProductManager>();

#endregion

#region Logging Configuration

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log_requests-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
