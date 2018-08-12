using System.Threading.Tasks;

namespace BasicMessageStore.Models.Security
{
    public interface IUserRepository : IRepository<User.User>
    {
        /// <summary>
        /// Tries to login a user using the provided username and password
        /// </summary>
        /// <returns>true is successful</returns>
        Task<bool> Login(string username, string password);

        Task<User.User> GetByUsername(string username);
    }
}