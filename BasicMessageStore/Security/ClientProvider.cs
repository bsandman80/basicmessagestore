namespace BasicMessageStore.Security
{
    public class CurrentClientProvider : ICurrentClientProvider
    {
        public string CurrentUser { get; set; }
    }
}