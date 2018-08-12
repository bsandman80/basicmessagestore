using BasicMessageStore.Models.Users;

namespace BasicMessageStore.Security
{
    /// <summary>
    /// Provides basic information about current client
    /// </summary>
    public interface IClientProvider
    {
        User CurrentUser { get; set; }
    }
}