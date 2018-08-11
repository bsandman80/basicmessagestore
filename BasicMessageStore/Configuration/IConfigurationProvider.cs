namespace BasicMessageStore.Configuration
{
    public interface IConfigurationProvider
    {
        string ConnectionString { get; }
        string TokenSecret { get; }
        int TokenExpirationMinutes { get; }
        string TokenIssuer { get; }
        string TokenAudience { get; }
    }
}