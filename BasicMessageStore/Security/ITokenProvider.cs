using BasicMessageStore.Models.Users;

namespace BasicMessageStore.Security
{
    public interface ITokenProvider
    {                
        /// <summary>
        /// Generates a new access token for given user
        /// </summary>
        string GenerateAccessToken(User user);
    }
}