using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenMiddleWare;
using CanteenManage.Models;
using CanteenManage.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using static NuGet.Packaging.PackagingConstants;
using Microsoft.IdentityModel.Tokens;
using CanteenManage.Middleware;
using CanteenManage.Utility;

var builder = WebApplication.CreateBuilder(args);

//
var projectFolder = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "CanteenManagementSystem");

if (!Directory.Exists(projectFolder))
{
    Directory.CreateDirectory(projectFolder);
}
AppConfigs appConfigs = new AppConfigs();
//if (File.Exists(Path.Combine(projectFolder, "AppConfigs.json")))
{
    try
    {

        var appConfigJson = File.ReadAllText(Path.Combine(projectFolder, "AppConfigs.json"));
        appConfigs = System.Text.Json.JsonSerializer.Deserialize<AppConfigs>(appConfigJson) ?? new AppConfigs();
        builder.Environment.EnvironmentName = appConfigs.getAppEnvironment();

        AppConfigs appconff = appConfigs.getEncryptedObject();
        var appConfigJson2 = System.Text.Json.JsonSerializer.Serialize(appconff);


    }
    catch (Exception ex)
    {
        throw;
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = appConfigs.getTokenIssuer(),
        ValidAudience = appConfigs.getTokenAudience(),
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appConfigs.getSecretKey()))
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            // Handle token validation failures (e.g., invalid token)
            if (context.Exception is SecurityTokenInvalidSignatureException)
            {
                context.Fail("Invalid signature");
            }
            else if (context.Exception is SecurityTokenExpiredException)
            {
                context.Fail("Token expired");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddPooledDbContextFactory<CanteenManageDBContext>(option =>
{
    //option.UseSqlServer(builder.Configuration.GetConnectionString("CantenSystemDBConnection"));
    option.UseSqlServer(appConfigs.getConnectionString());
});

builder.Services.AddScoped<CanteenManageContextFactory>();
builder.Services.AddScoped(sp => sp.GetRequiredService<CanteenManageContextFactory>().CreateDbContext());
builder.Services.AddScoped<AppConfigProvider>();
builder.Services.AddScoped<UtilityServices>();
builder.Services.AddTransient<LoginService>();
builder.Services.AddScoped<FoodListingService>();
builder.Services.AddScoped<OrderingService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

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

app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseCors(MyAllowSpecificOrigins);

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

app.UseMiddleware<TokenAuthMiddleWare>();


app.UseAuthentication();
app.UseAuthorization();
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
