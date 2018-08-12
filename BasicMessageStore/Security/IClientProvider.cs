namespace BasicMessageStore.Security
{
    public interface ICurrentClientProvider
    {
        string CurrentUser { get; }
    }
}