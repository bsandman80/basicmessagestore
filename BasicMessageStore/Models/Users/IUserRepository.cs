using System.Threading.Tasks;

namespace BasicMessageStore.Models.Users
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Checks credentials for a user
        /// </summary>
        /// <returns>true is successful</returns>
        Task<bool> Login(string username, string password);

        Task<User> GetByUsername(string username);
    }
}