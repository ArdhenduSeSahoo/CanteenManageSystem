using CanteenManage.Repo.Contexts;
using CanteenManage.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddPooledDbContextFactory<CanteenManageDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("CantenSystemDBConnection"));
});
//builder.Services.AddDbContext<CanteenManageDBContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("CantenSystemDBConnection")));
builder.Services.AddScoped<CanteenManageContextFactory>();
builder.Services.AddScoped(sp => sp.GetRequiredService<CanteenManageContextFactory>().CreateDbContext());
builder.Services.AddScoped<OrderingService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();
//app.Use(async (context, next) =>
//{
//    if (context.Session.GetString("UserId") == null)
//    {
//        context.Response.Redirect("/");
//        return;
//    }
//    await next();
//});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
