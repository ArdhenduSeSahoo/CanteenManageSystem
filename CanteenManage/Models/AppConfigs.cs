namespace CanteenManage.Models
{
    public class AppConfigs
    {
        public string ConnectionString { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string TokenIssuer { get; set; } = "";
        public string TokenAudience { get; set; } = "";
        //public int TokenExpirationMinutes { get; set; }
        //public string RedisConnectionString { get; set; }
        //public string RedisInstanceName { get; set; }
        //public string RedisPassword { get; set; }
        //public int RedisDatabaseId { get; set; }
        //public string RedisKeyPrefix { get; set; }
        //public string RedisKeySeparator { get; set; }
    }
}
