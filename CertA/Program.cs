using CertA.Data;
using CertA.Models;
using CertA.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((ctx, lc) =>
{
    var conn = ctx.Configuration.GetConnectionString("DefaultConnection");
    var columnWriters = new Dictionary<string, ColumnWriterBase>
    {
        ["id"] = new SerialColumnWriter(),
        ["timestamp"] = new TimestampColumnWriter(),
        ["level"] = new LevelColumnWriter(renderAsText: true),
        ["message"] = new MessageTemplateColumnWriter(),
        ["message_rendered"] = new RenderedMessageColumnWriter(),
        ["exception"] = new ExceptionColumnWriter(),
        ["properties"] = new LogEventSerializedColumnWriter(),
        ["event_id"] = new EventIdColumnWriter()
    };

    lc.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
      .MinimumLevel.Override("System", LogEventLevel.Warning)
      .MinimumLevel.Information()
      .WriteTo.Console()
      .WriteTo.PostgreSQL(
          connectionString: conn!,
          tableName: "application_logs",
          columnOptions: columnWriters,
          needAutoCreateTable: true);
});

builder.Services.AddControllersWithViews();

// Add Entity Framework
builder.Services.AddDbContext<CertA.Data.AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(12);
    options.SlidingExpiration = true;
});

// Add services
builder.Services.AddScoped<CertA.Services.ICertificateService, CertA.Services.CertificateService>();
builder.Services.AddScoped<CertA.Services.ICertificateAuthorityService, CertA.Services.CertificateAuthorityService>();

var app = builder.Build();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Create default admin user if no users exist
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    if (!userManager.Users.Any())
    {
        var adminUser = new ApplicationUser
        {
            UserName = "admin@certa.local",
            Email = "admin@certa.local",
            FirstName = "Admin",
            LastName = "User",
            Organization = "CertA",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            Console.WriteLine("Default admin user created: admin@certa.local / Admin123!");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();