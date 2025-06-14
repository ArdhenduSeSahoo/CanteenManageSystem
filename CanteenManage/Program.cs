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
using Serilog.Events;
using Microsoft.CodeAnalysis.Elfie.Serialization;



var projectFolder = CustomDataConstants.ProjectFolder;
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    //Log.Information("Starting up the application");


    var builder = WebApplication.CreateBuilder(args);

    if (builder.Environment.IsProduction())
    {
        projectFolder = builder.Environment.ContentRootPath + "\\CMS_Files";
        CustomDataConstants.ProjectFolder = builder.Environment.ContentRootPath + "\\CMS_Files";
    }

    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.WriteTo.Console();

        loggerConfiguration.MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("System", LogEventLevel.Warning);
        loggerConfiguration.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information);
        loggerConfiguration.WriteTo.File(projectFolder + "\\Logs" + "\\log-.txt",
            rollingInterval: RollingInterval.Day,
            restrictedToMinimumLevel: LogEventLevel.Error
            );
        //loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });

    AppConfigs appConfigs = new AppConfigs();

    if (!Directory.Exists(projectFolder))
    {
        Directory.CreateDirectory(projectFolder);
    }

    //if (File.Exists(Path.Combine(projectFolder, "AppConfigs.json")))
    {

        var appConfigJson = File.ReadAllText(Path.Combine(projectFolder, "AppConfigs.json"));
        appConfigs = System.Text.Json.JsonSerializer.Deserialize<AppConfigs>(appConfigJson) ?? new AppConfigs();
        builder.Environment.EnvironmentName = appConfigs.getAppEnvironment();
        //var con = appConfigs.getConnectionString();
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
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appConfigs.getSecretKey()))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                // Handle token validation failures (e.g., invalid token)
                //if (context.Exception is SecurityTokenInvalidSignatureException)
                //{
                //    context.Fail("Invalid signature");
                //}
                //else if (context.Exception is SecurityTokenExpiredException)
                //{
                //    context.Fail("Token expired");
                //}
                context.Response.Redirect("/Error");
                return Task.CompletedTask;
            }
        };
    });

    builder.Services.AddPooledDbContextFactory<CanteenManageDBContext>(option =>
    {
        //option.UseSqlServer(builder.Configuration.GetConnectionString("CantenSystemDBConnection"));
        option.UseSqlServer(appConfigs.getConnectionString());
        if (builder.Environment.IsDevelopment())
        {
            option.LogTo(Console.WriteLine, LogLevel.Information)
                  .EnableSensitiveDataLogging()
                  .EnableDetailedErrors();
        }

    });

    builder.Services.AddHttpClient(CustomDataConstants.PortalAuthValidater, httpClient =>
    {
        httpClient.BaseAddress = new Uri(appConfigs.PortalAuthValidaTorBaseURL);
        httpClient.DefaultRequestHeaders.Clear();

    }).SetHandlerLifetime(TimeSpan.FromHours(5));

    builder.Services.AddSingleton<SignalRDataHolder>();
    builder.Services.AddSingleton<SessionManager>();
    builder.Services.AddSingleton<AppConfigProvider>();
    builder.Services.AddSignalR(e =>
    {
        e.EnableDetailedErrors = true;
        e.MaximumReceiveMessageSize = 1024000; // Set maximum message size to 1 MB
    });
    //builder.Services.AddHostedService<SignalRBackgroundService>();
    builder.Services.AddScoped<CanteenManageContextFactory>();
    builder.Services.AddScoped(sp => sp.GetRequiredService<CanteenManageContextFactory>().CreateDbContext());

    builder.Services.AddScoped<UtilityServices>();

    builder.Services.AddTransient<LoginService>();
    builder.Services.AddScoped<FoodListingService>();
    builder.Services.AddScoped<OrderingService>();
    builder.Services.AddScoped<CartService>();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(120);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

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

    app.UseMiddleware<ErrorHandlerMiddleWare>();
    app.UseCors(MyAllowSpecificOrigins);
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


    app.UseRouting();

    app.UseSession();

    app.UseMiddleware<TokenAuthMiddleWare>();


    app.UseAuthentication();
    app.UseAuthorization();

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
