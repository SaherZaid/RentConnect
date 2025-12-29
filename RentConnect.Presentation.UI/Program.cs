using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;

using RentConnect.API.RentConnect.Infrastructure.Repositories;

//using RentConnect.API.SignalR_Hub;
using RentConnect.Presentation.UI;
using RentConnect.Presentation.UI.Components;
using RentConnect.Presentation.UI.Components.Account;
using RentConnect.Presentation.UI.Data;
using RentConnect.Presentation.UI.IServices;
using RentConnect.Presentation.UI.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();



builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();


//builder.Services.AddScoped<INotificationService, NotificationService>();

// Add email sender service
builder.Services.AddScoped<IEmailSender<ApplicationUser>, EmailSender>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationApi, NotificationApi>();

//Services

builder.Services.AddHttpClient<NotificationApi>(c =>
{
    c.BaseAddress = new Uri("https://localhost:7087/");
});
builder.Services.AddScoped<INotificationApi, NotificationApi>();

builder.Services.AddHttpClient<IReportService, ReportService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7087/");
});

builder.Services.AddHttpClient<IReportAdminService, ReportAdminService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7087/");
});

builder.Services.AddHttpClient<IItemService, ItemService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7087/");
});
builder.Services.AddHttpClient<ICategoryService, CategoryService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7087/");
});

builder.Services.AddHttpClient<IBookingService, BookingService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7087/");
});

builder.Services.AddHttpClient<IReviewService, ReviewService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7087/");
});

builder.Services.AddHttpClient<IFavoriteService, FavoriteService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7087/");
});

builder.Services.AddHttpClient<UserService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7087/");
});

builder.Services.AddHttpClient<IChatService, ChatService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7087/");
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5078") });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configure password settings
    var identityOptions = builder.Configuration.GetSection("IdentityOptions:Password");
    options.Password.RequireDigit = identityOptions.GetValue<bool>("RequireDigit");
    options.Password.RequireLowercase = identityOptions.GetValue<bool>("RequireLowercase");
    options.Password.RequireUppercase = identityOptions.GetValue<bool>("RequireUppercase");
    options.Password.RequiredLength = identityOptions.GetValue<int>("RequiredLength");
    options.Password.RequireNonAlphanumeric = identityOptions.GetValue<bool>("RequireNonAlphanumeric");
    options.Password.RequiredUniqueChars = identityOptions.GetValue<int>("RequiredUniqueChars");

    // Sign-in options
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure redirect paths for unauthorized access
builder.Services.ConfigureApplicationCookie(cookieOptions =>
{
    var identityCookieOptions = builder.Configuration.GetSection("IdentityOptions");
    cookieOptions.LoginPath = identityCookieOptions.GetValue<string>("LoginPath");
    cookieOptions.AccessDeniedPath = identityCookieOptions.GetValue<string>("AccessDeniedPath");
});

// Register RoleInitializer as a scoped service
builder.Services.AddScoped<RoleInitializer>();


//builder.Services.AddJSRuntime();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 200 * 1024 * 1024;
});

builder.Services.AddScoped<UserService>();

builder.Services.AddHttpClient(); // just basic HttpClient
builder.Services.AddHttpContextAccessor();

//builder.Services.AddMudServices();

var app = builder.Build();

// Initialize roles (if needed)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleInitializer = services.GetRequiredService<RoleInitializer>();
    await roleInitializer.SeedRolesAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

//app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseStaticFiles();


//app.MapHub<ChatHub>("/chathub");
app.MapAdditionalIdentityEndpoints();

//app.MapBlazorHub();

app.Run();



