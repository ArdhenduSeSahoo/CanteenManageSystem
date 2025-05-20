using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.Models;
using CanteenManage.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using static NuGet.Packaging.PackagingConstants;

var builder = WebApplication.CreateBuilder(args);

//
var projectFolder = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "CanteenManagementSystem");

if (!Directory.Exists(projectFolder))
{
    Directory.CreateDirectory(projectFolder);
}
AppConfigs appConfigs = new AppConfigs();
if (File.Exists(Path.Combine(projectFolder, "AppConfigs.json")))
{
    try
    {
        var appConfigJson = File.ReadAllText(Path.Combine(projectFolder, "AppConfigs.json"));
        appConfigs = System.Text.Json.JsonSerializer.Deserialize<AppConfigs>(appConfigJson) ?? new AppConfigs();
    }
    catch (Exception ex)
    {

    }

}
//else
//{
//    File.WriteAllText(Path.Combine(projectFolder, "AppConfigs.json"), System.Text.Json.JsonSerializer.Serialize(appConfigs));
//}


//
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
    //option.UseSqlServer(builder.Configuration.GetConnectionString("CantenSystemDBConnection"));
    option.UseSqlServer(appConfigs.ConnectionString);
});

builder.Services.AddScoped<CanteenManageContextFactory>();
builder.Services.AddScoped(sp => sp.GetRequiredService<CanteenManageContextFactory>().CreateDbContext());
builder.Services.AddScoped<UtilityServices>();
builder.Services.AddScoped<FoodListingService>();
builder.Services.AddScoped<OrderingService>();
builder.Services.AddScoped<CartService>();


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin();
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();

                      });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
           Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\CanteenManagementSystem", "FoodImages")),
        RequestPath = "/FoodImage"
    });
app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
           Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\CanteenManagementSystem", "images")),
        RequestPath = "/images"
    });
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
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
