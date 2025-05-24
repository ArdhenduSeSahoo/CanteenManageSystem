using CanteenManage.Models;
using CanteenManage.Utility;

namespace CanteenManage.Services
{
    public class AppConfigProvider
    {
        AppConfigs? appConfigs = new AppConfigs();
        private readonly IConfiguration _configuration;
        public AppConfigProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            var projectFolder = CustomDataConstants.ProjectFolder;
            if (!Directory.Exists(projectFolder))
            {
                Directory.CreateDirectory(projectFolder);
            }

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
            //appConfigs = new AppConfigs().getDefaultObject();
        }
        public string? GetConnectionString()
        {
            return appConfigs?.getConnectionString();
        }
        public string? GetSecretKey()
        {
            return appConfigs?.getSecretKey();
        }
        public string? GetTokenIssuer()
        {
            return appConfigs?.getTokenIssuer();
        }
        public string? GetTokenAudience()
        {
            return appConfigs?.getTokenAudience();
        }

        public bool IsDevelopment()
        {
            if (appConfigs?.getAppEnvironment() == "Development")
            {
                return true;
            }
            else
            {
                return false;
            }
            //return appConfigs?.getTokenAudience();
        }

    }
}
