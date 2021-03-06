﻿using System;
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
        private IPasswordHasher<User> _passwordHasher;
        public MessageStoreContext Context { get; set; }
        
        public UserRepository(MessageStoreContext context, IPasswordHasher<User> passwordHasher)
        {   
            Context = context;
            _passwordHasher = passwordHasher;
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
            
            model.HashedPassword = _passwordHasher.HashPassword(model, model.Password);
            model.Password = null;
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
            
            //TODO: This should probably delete all messages created by user as well
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
            var user = await Context.Users.Where(b => b.Id == id).SingleOrDefaultAsync();
            if (user == null)
                throw new MessageStoreException(ErrorCodes.ResourceNotFound, "User could not be found");
            return user;
        }

        public async Task<bool> Login(string username, string password)
        {
            var user = await GetByUsername(username);
            if (user == null || String.IsNullOrWhiteSpace(password))
                return false;
            var result = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password);
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