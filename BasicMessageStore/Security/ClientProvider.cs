using BasicMessageStore.Models.Users;

namespace BasicMessageStore.Security
{
    public class ClientProvider : IClientProvider
    {
        public User CurrentUser { get; set; }
    }
}