using Microsoft.EntityFrameworkCore;
using trackingg.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

// Add authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
        googleOptions.CallbackPath = "/signin-google"; // Make sure this matches your redirect URI in Google Console
        // Add this line to set "Google" as "External" for backward compatibility
        googleOptions.SignInScheme = "External";
    })
    .AddCookie("External", options => 
    {
        options.Cookie.Name = "External";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    });
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<IssueDbContext>(o => o.UseSqlite("filename=Data/Database/Issue.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // Enable HTTPS in production
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.EnsureDatabaseCreated();
app.Run();