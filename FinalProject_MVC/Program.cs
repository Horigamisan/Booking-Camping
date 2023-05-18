using FinalProject_MVC.Middlewares;
using FinalProject_MVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<AccessTokenInterceptor>();

// Thêm HttpClient
builder.Services.AddHttpClient("WebApi", c =>
{
    c.BaseAddress = new Uri("https://localhost:7001");
}).AddHttpMessageHandler<AccessTokenInterceptor>();

builder.Services.AddHttpClient("Mailer", c =>
{
    c.BaseAddress = new Uri("http://localhost:5000");
});

builder.Services.AddTransient<IWebAPIService, WebAPIService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/accessdenied";

        options.Cookie.Name = "FinalProject";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.SlidingExpiration = true;

        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // thiết lập thời gian timeout cho session
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/notfound";
        await next();
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
