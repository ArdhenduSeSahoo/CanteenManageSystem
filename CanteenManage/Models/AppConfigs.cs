﻿using CanteenManage.Utility;

namespace CanteenManage.Models
{
    public class AppConfigs
    {

        public string ConnectionString { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string TokenIssuer { get; set; } = "";
        public string TokenAudience { get; set; } = "";
        public string AppEnvironment { get; set; } = "";
        public string LogOutURL { get; set; } = "";
        public string PortalAuthValidaTorBaseURL { get; set; } = "";
        public string PortalAuthValidaTorEndpoint { get; set; } = "";

        /////for Production
        public string qpiowerbzlkvywe34bdsdvx0zx { get; set; } = "";
        public string hgwf899fwMi66chz394ghz { get; set; } = "";
        public string mzbcbjziahfiapehag { get; set; } = "";
        public string bzbdwywuila83kfkdsjh { get; set; } = "";
        public string vtauerbzdchzpobqwjqbvgdyewwt { get; set; } = "";
        public string getConnectionString()
        {
            return ConnectionString != "" ? ConnectionString : new EncryptionDecryptions().DecryptString(qpiowerbzlkvywe34bdsdvx0zx);
        }
        public string getSecretKey()
        {
            return SecretKey != "" ? SecretKey : new EncryptionDecryptions().DecryptString(hgwf899fwMi66chz394ghz);
        }
        public string getTokenIssuer()
        {
            return TokenIssuer != "" ? TokenIssuer : new EncryptionDecryptions().DecryptString(mzbcbjziahfiapehag);
        }
        public string getTokenAudience()
        {
            return TokenAudience != "" ? TokenAudience : new EncryptionDecryptions().DecryptString(bzbdwywuila83kfkdsjh);
        }
        public string getAppEnvironment()
        {
            return AppEnvironment != "" ? AppEnvironment : new EncryptionDecryptions().DecryptString(vtauerbzdchzpobqwjqbvgdyewwt);
        }
        public string getLogOutURL()
        {
            return LogOutURL;
        }
        public AppConfigs getEncryptedObject()
        {
            AppConfigs appConfigs = new AppConfigs();
            appConfigs.ConnectionString = ConnectionString;
            appConfigs.SecretKey = SecretKey;
            appConfigs.TokenIssuer = TokenIssuer;
            appConfigs.TokenAudience = TokenAudience;
            appConfigs.AppEnvironment = AppEnvironment;
            appConfigs.LogOutURL = LogOutURL;
            //for Production
            appConfigs.qpiowerbzlkvywe34bdsdvx0zx = new EncryptionDecryptions().EncryptString(ConnectionString);
            appConfigs.hgwf899fwMi66chz394ghz = new EncryptionDecryptions().EncryptString(SecretKey);
            appConfigs.mzbcbjziahfiapehag = new EncryptionDecryptions().EncryptString(TokenIssuer);
            appConfigs.bzbdwywuila83kfkdsjh = new EncryptionDecryptions().EncryptString(TokenAudience);
            appConfigs.vtauerbzdchzpobqwjqbvgdyewwt = new EncryptionDecryptions().EncryptString(AppEnvironment);
            return appConfigs;

        }
        public AppConfigs getDefaultObject()
        {
            return new AppConfigs()
            {
                ConnectionString = "Data Source=192.168.0.83\\SQLSERVER2022;Initial Catalog=CanteenMgmt; User ID=CanteenUser;password=CanteenUser;TrustServerCertificate=True;",
                SecretKey = "askdypwesdtd6u543335465u6yfoywtmcvyw]toi437sddvf843543vxnsrgaht45jdvle34tV879iuraoylaeg",
                TokenIssuer = "CanteenMgtSyt",
                TokenAudience = "CanteenEmploy",
                AppEnvironment = "Production",

            };
        }
    }
}
