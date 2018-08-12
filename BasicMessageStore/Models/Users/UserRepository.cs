using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasicMessageStore.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BasicMessageStore.Models.Users
{
    public class UserRepository : IUserRepository
    {
        public MessageStoreContext Context { get; set; }
        
        public UserRepository(MessageStoreContext context)
        {   
            Context = context;
        }

        public async Task<User> AddAsync(User model)
        {
            // TODO: Implement attributes for validation to avoid these kind of checks
            if (String.IsNullOrWhiteSpace(model.Username))
                throw new MessageStoreException(ErrorCodes.Required, "Username is required");
            if (String.IsNullOrWhiteSpace(model.Password))
                throw new MessageStoreException(ErrorCodes.Required, "Password is required");

            if (await UsernameExists(model.Username))
                throw new MessageStoreException(ErrorCodes.Unique, "Username already exists");
            
            var passwordHasher = new PasswordHasher<User>();
            model.Password = passwordHasher.HashPassword(model, model.Password);            
            Context.Users.Add(model);
            await Context.SaveChangesAsync();
            return model;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id); 
            if (user == null)
                throw new MessageStoreException(ErrorCodes.ResourceNotFound, "User could not be found");
            Context.Users.Remove(user);
            await Context.SaveChangesAsync();
        }

        public Task UpdateAsync(User model)
        {
            // TODO: Implement update of users
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetAsync()
        {
            return await Context.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var user = await Context.Users.FindAsync(id);
            if (user == null)
                throw new MessageStoreException(ErrorCodes.ResourceNotFound, "User could not be found");
            return user;
        }

        public async Task<bool> Login(string username, string password)
        {
            var user = await GetByUsername(username);
            if (user == null || String.IsNullOrWhiteSpace(password))
                return false;
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }

        public async Task<User> GetByUsername(string username)
        {
            var users = await GetAsync();
            var user =  users.FirstOrDefault(x => x.Username == username);
            if (user == null)
                throw new MessageStoreException(ErrorCodes.ResourceNotFound, "User could not be found");
            return user;
        }

        public async Task<bool> UsernameExists(string username)
        {
            var users = await GetAsync();
            return users.FirstOrDefault(x => x.Username == username) != null;
        }
    }
}