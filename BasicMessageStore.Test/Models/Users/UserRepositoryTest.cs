using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BasicMessageStore.Exceptions;
using BasicMessageStore.Models;
using BasicMessageStore.Models.Users;
using BasicMessageStore.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using NUnit.Framework;

namespace BasicMessageStore.Test.Models.Users
{
  public class UserRepositoryTest : RepositoryTestCommon
  {        
    private (UserRepository repo, Mock<IPasswordHasher<User>> hasher) SetupTarget(params User[] existingUsers)
    {      
      var clientProvider = new Mock<IClientProvider>();

      var context = GetContext(clientProvider.Object);
      
      foreach (var user in existingUsers)
        context.Users.Add(user);
      if (existingUsers.Any())
        context.SaveChanges();
      
      var hasher = new Mock<IPasswordHasher<User>>();
      var repo = new UserRepository(context, hasher.Object);
      return (repo, hasher);            
    }
    
    [Test]
    public async Task AddAsync_MissingUsernameThrows()
    {      
      var (repo, hasher) = SetupTarget();

      var user = new User {Username = String.Empty, Password = "Password"};

      var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.AddAsync(user));

      Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.Required));
    }
    
    [Test]
    public async Task AddAsync_MissingPasswordThrows()
    {      
      var (repo, hasher) = SetupTarget();

      var user = new User {Username = "Username", Password = ""};

      var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.AddAsync(user));

      Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.Required));
    }
    
    [Test]
    public async Task AddAsync_UserExistsThrows()
    { 
      var user = new User {Username = "ExistingUser", Password = "Password"};
      var (repo, hasher) = SetupTarget(user);

      var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.AddAsync(user));

      Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.Unique));
    }
    
    [Test]
    public async Task AddAsync_PasswordIsNotStoredAsClearText()
    {       
      var (repo, hasher) = SetupTarget();

      var user = new User {Username = "Username", Password = "Password"};
      
      const string hashedPassword = "HashedPassword";      
      hasher.Setup(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns(hashedPassword);            
      
      var savedUser = await repo.AddAsync(user);
      Assert.That(savedUser.Password, Is.EqualTo(hashedPassword));
    }    
    
    [Test]
    public async Task AddAsync_UserIsSavedToDatabase()
    {       
      var (repo, hasher) = SetupTarget();

      var user = new User {Username = "Username", Password = "Password"};                
      
      var userResult = await repo.AddAsync(user);

      var savedUser = repo.Context.Users.FirstOrDefault(x => x.Id == userResult.Id);
      Assert.IsNotNull(savedUser);
      Assert.AreEqual(savedUser.Username, user.Username);
      Assert.AreEqual(savedUser.Password, user.Password);
    }

    [Test]
    public async Task DeleteAsync_DeleteNonExistingUserThrows()
    {
      var (repo, hasher) = SetupTarget();

      const int NonExistingUserId = 15;

      var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.DeleteAsync(NonExistingUserId));

      Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.ResourceNotFound));
    }

    [Test]
    public async Task DeleteAsync_DeletedUserIsRemovedFromDatabase()
    {
      var user = new User {Id = 1, Username = "Username", Password = "Password"};
      var (repo, hasher) = SetupTarget(user);
                
      await repo.DeleteAsync(user.Id);

      var deletedUser = repo.Context.Users.FirstOrDefault(x => x.Id == user.Id);
      Assert.IsNull(deletedUser);
    }
    
    [Test]
    public async Task GetAsync_ReturnsAllUsers()
    {
      var users = new List<User>
      {
        new User {Username = "user1", Password = "pass1"},
        new User {Username = "user2", Password = "pass2"}
      };
      
      var (repo, hasher) = SetupTarget(users.ToArray());

      var usersInRepo = await repo.GetAsync();
      
      Assert.IsTrue(users.All(usersInRepo.Contains));
      Assert.IsTrue(usersInRepo.All(users.Contains));
    }

    [Test]
    public async Task GetByIdAsync_NonExistingUserThrows()
    {
      var (repo, hasher) = SetupTarget();

      const int NonExistingUserId = 15;

      var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.GetByIdAsync(NonExistingUserId));

      Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.ResourceNotFound));
    }

    [Test]
    public async Task Login_NonExistingUserThrows()
    {
      var (repo, hasher) = SetupTarget();

      const string NonExistingUsername = "NonExistingUser";

      var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.Login(NonExistingUsername, "password"));

      Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.ResourceNotFound));
    }
   
    [Test]
    public async Task Login_EmptyPasswordReturnsFalse()
    {
      var user = new User {Id = 1, Username = "Username", Password = "Password"};
      var (repo, hasher) = SetupTarget(user);

      var ret = await repo.Login(user.Username, String.Empty);

      Assert.IsFalse(ret);
    }
    
    [Test]
    public async Task Login_ExistingUserAndPasswordReturnsTrue()
    {      
      var (repo, hasher) = SetupTarget();

      const string hashedPassword = "HashedPassword";      
      hasher.Setup(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns(hashedPassword);
      hasher.Setup(x => x.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).Returns(PasswordVerificationResult.Success);
      
      var user = new User {Id = 1, Username = "Username", Password = "Password"};
      repo.Context.Users.Add(user);
      repo.Context.SaveChanges();
      
      var ret = await repo.Login(user.Username, user.Password);

      Assert.IsTrue(ret);
    }

    [Test]
    public async Task GetByUsername_NonExistingUsernameThrows()
    {
      var (repo, hasher) = SetupTarget();

      const string NonExistingUsername = "NonExistingUser";

      var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.GetByUsername(NonExistingUsername));

      Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.ResourceNotFound));
    }
    
    [Test]
    public async Task GetByUsername_ReturnsUserWithUsername()
    {
      var user = new User {Id = 1, Username = "Username", Password = "Password"};
      var (repo, hasher) = SetupTarget(user);

      var existing = await repo.GetByUsername(user.Username);
      
      Assert.AreEqual(existing.Username, user.Username);
      Assert.AreEqual(existing.Id, user.Id);
    }
    
    [Test]
    public async Task UsernameExists_ReturnsTrueForExistingUser()
    {
      var user = new User {Id = 1, Username = "Username", Password = "Password"};
      var (repo, hasher) = SetupTarget(user);

      var exists = await repo.UsernameExists(user.Username);
      
      Assert.IsTrue(exists);
    }
    
    [Test]
    public async Task UsernameExists_ReturnsFalseForNonExistingUser()
    {
      var (repo, hasher) = SetupTarget();
      const string NonExistingUsername = "NonExistingUser";
      var exists = await repo.UsernameExists(NonExistingUsername);
      
      Assert.IsFalse(exists);
    }
  }
}