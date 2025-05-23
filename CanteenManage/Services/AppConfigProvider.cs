using CanteenManage.Models;

namespace CanteenManage.Services
{
    public class AppConfigProvider
    {
        AppConfigs? appConfigs = new AppConfigs();

        public AppConfigProvider()
        {

            var projectFolder = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "CanteenManagementSystem");

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
