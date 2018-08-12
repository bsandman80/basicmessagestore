using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BasicMessageStore.Models.Security
{
    public class UserRepository : IUserRepository
    {
        public MessageStoreContext Context { get; set; }
        
        public UserRepository(MessageStoreContext context )
        {
            Context = context;
        }

        public async Task<User.User> AddAsync(User.User model)
        {
            var passwordHasher = new PasswordHasher<User.User>();
            model.Password = passwordHasher.HashPassword(model, model.Password);            
            Context.Users.Add(model);
            await Context.SaveChangesAsync();
            return model;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);            
            Context.Users.Remove(user);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User.User model)
        {
            Context.Users.Update(model);
            await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User.User>> GetAsync()
        {
            return await Context.Users.ToListAsync();
        }

        public async Task<User.User> GetByIdAsync(int id)
        {
            return await Context.Users.FindAsync(id);
        }

        public async Task<bool> Login(string username, string password)
        {
            var user = await GetByUsername(username);
            if (user == null)
                return false;
            var passwordHasher = new PasswordHasher<User.User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }

        public async Task<User.User> GetByUsername(string username)
        {
            var users = await GetAsync();
            return users.FirstOrDefault(x => x.Username == username);
        }
    }
}