using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using System.Text.Json.Serialization;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.SignalR_Hub;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;
using RentConnect.API.RentConnect.Infrastructure.Repositories;
using RentConnect.API.RentConnect.Infrastructure.Services;
using RentConnect.API.IService;

//using RentConnect.API.SignalR_Hub;

//using RentConnect.API.RentConnect.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Use ReferenceHandler.IgnoreCycles to avoid $id and $values
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true; // Optional: Pretty-print JSON output
    });

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services for ApplicationUser and IdentityRole
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApiDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHostedService<UnreadMessageReminderService>();
// Register UnitOfWork and Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<SwishPaymentService>();
builder.Services.AddScoped<IEmailSender<ApplicationUser>, EmailSender>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();





builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7052") // ???? Blazor Server
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});



//// Add JWT Authentication
//builder.Services.AddAuthentication("Bearer")
//    .AddJwtBearer("Bearer", options =>
//    {
//        options.Authority = "https://your-auth-provider.com"; // Replace this with your authentication provider's URL
//        options.Audience = "your-api-audience"; // Replace this with your API's audience
//    });

// Add Authorization
builder.Services.AddAuthorization();


builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMemoryCache();

//builder.Services.AddSwaggerGen(c =>
//{
//    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT",
//        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
//        Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'"
//    });

//    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
//    {
//        {
//            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//            {
//                Reference = new Microsoft.OpenApi.Models.OpenApiReference
//                {
//                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});


var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.MapHub<ChatHub>("/chathub");

app.UseCors();
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

// Use authentication middleware
app.UseAuthentication(); // Add this line to enable authentication

app.UseAuthorization();




app.MapControllers();
app.MapHub<NotificationHub>("/notificationhub");

app.Run();



