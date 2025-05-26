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
using System.Configuration;
using Serilog;
using CanteenManage.Controllers;



var projectFolder = CustomDataConstants.ProjectFolder;
//Log.Logger = new LoggerConfiguration()
//    //.WriteTo.Console()
//    .WriteTo.File(projectFolder + "\\Logs" + "\\log-.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();

try
{
    //Log.Information("Starting up the application");


    var builder = WebApplication.CreateBuilder(args);


    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        //loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });

    if (builder.Environment.IsProduction())
    {
        projectFolder = builder.Environment.ContentRootPath + "\\CMS_Files";
        CustomDataConstants.ProjectFolder = builder.Environment.ContentRootPath + "\\CMS_Files";
    }
    Log.Logger = new LoggerConfiguration()
        //.WriteTo.Console()
        .WriteTo.File(projectFolder + "\\Logs" + "\\log-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    AppConfigs appConfigs = new AppConfigs();

    // var projectFolder = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "CMS_Files");

    //var projectFolder = builder.Configuration["ProjectDir"];


    var propath = Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);


    if (!Directory.Exists(projectFolder))
    {
        Directory.CreateDirectory(projectFolder);
    }

    //if (File.Exists(Path.Combine(projectFolder, "AppConfigs.json")))
    {

        var appConfigJson = File.ReadAllText(Path.Combine(projectFolder, "AppConfigs.json"));
        appConfigs = System.Text.Json.JsonSerializer.Deserialize<AppConfigs>(appConfigJson) ?? new AppConfigs();
        builder.Environment.EnvironmentName = appConfigs.getAppEnvironment();
        var con = appConfigs.getConnectionString();
        //Log.Information("Config read form file");
        //AppConfigs appconff = appConfigs.getEncryptedObject();
        //var appConfigJson2 = System.Text.Json.JsonSerializer.Serialize(appconff);




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

    builder.Services.AddSingleton<SignalRDataHolder>();
    builder.Services.AddSignalR(e =>
    {
        e.EnableDetailedErrors = true;
        e.MaximumReceiveMessageSize = 1024000; // Set maximum message size to 1 MB
    });
    builder.Services.AddHostedService<SignalRBackgroundService>();
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
    //Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\CanteenManagementSystem"
    app.UseStaticFiles(
        new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
              Path.Combine(projectFolder, "FoodImages")),
            RequestPath = "/FoodImage"
        });
    app.UseStaticFiles(
        new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
               Path.Combine(projectFolder, "images")),
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

    app.MapHub<OrderingHub>("/OrderingHub");
    app.MapControllers();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Login}/{action=Index}/{id?}");

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
