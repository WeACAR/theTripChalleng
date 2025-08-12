using Microsoft.EntityFrameworkCore;
using theTripChalleng.Data;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // (Optional if you're seeing timestamp issues)
AppContext.SetSwitch("Npgsql.EnableSslProtocols", true); // If SSL handshake is causing issues

// ✅ Force Npgsql to use IPv4 stack only
System.Net.ServicePointManager.Expect100Continue = true;
System.Net.ServicePointManager.UseNagleAlgorithm = false;
System.Net.ServicePointManager.DefaultConnectionLimit = 100;
System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSession();

builder.Services.AddControllersWithViews();
// Configure Entity Framework Core with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"
        ),
        providerOptions => {
            providerOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null
);
        });
});
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Details}/{id?}");

app.UseAuthorization();

//app.MapRazorPages();

app.Run();
