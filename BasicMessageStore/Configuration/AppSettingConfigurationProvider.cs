using Microsoft.Extensions.Configuration;

namespace BasicMessageStore.Configuration
{
    /// <summary>
    /// Simple configuration provider which provides settings stored in a appsettings file
    /// </summary>
    public class AppSettingConfigurationProvider : IConfigurationProvider
    {
        private readonly IConfiguration _configuration;
        private const int DefaultTokenExpirationMinutes = 20;

        public AppSettingConfigurationProvider(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public string ConnectionString => _configuration["Database:ConnectionString"];
        public string TokenSecret => _configuration["Security:SecretKey"];
        public string TokenIssuer => _configuration["Security:Issuer"];
        public string TokenAudience => _configuration["Security:Audience"];

        public int TokenExpirationMinutes => !int.TryParse(_configuration["Security:TokenExpirationMinutes"], out var ret) ? DefaultTokenExpirationMinutes : ret;
    }
}