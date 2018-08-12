using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasicMessageStore.Exceptions;
using BasicMessageStore.Models.Messages;
using BasicMessageStore.Models.Users;
using BasicMessageStore.Security;
using Moq;
using NUnit.Framework;

namespace BasicMessageStore.Test.Models.Messages
{
    public class MessageRepositoryTest : RepositoryTestCommon
    {
        private (MessageRepository repo, Mock<IClientProvider> clientProvider) SetupTarget(params Message[] existingMessages)
        {      
            var clientProvider = new Mock<IClientProvider>();
            var repo = new MessageRepository(GetContext(clientProvider.Object), clientProvider.Object);
            foreach (var message in existingMessages)
                repo.Context.Messages.Add(message);
            if (existingMessages.Any())
                repo.Context.SaveChanges();
            return (repo, clientProvider);            
        }

        [Test]
        public void AddAsync_MissingHeaderThrows()
        {
            var (repo, _) = SetupTarget();

            var message = new Message {Header = String.Empty, Body = "I am a body"};
            
            var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.AddAsync(message));

            Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.Required));

        }
        
        [Test]
        public void AddAsync_MissingBodyThrows()
        {
            var (repo, _) = SetupTarget();

            var message = new Message {Header = "I am a header", Body = String.Empty};
            
            var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.AddAsync(message));

            Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.Required));

        }
        
        [Test]
        public async Task AddAsync_MessageIsPersisted()
        {
            var (repo, _) = SetupTarget();

            var message = new Message {Header = "I am a header", Body = "I am a body"};
            
            message = await repo.AddAsync(message);

            var persistedMessage = repo.Context.Messages.FirstOrDefault(x => x.Id == message.Id);
            
            Assert.IsNotNull(persistedMessage);
            Assert.AreEqual(persistedMessage.Header, message.Header);
            Assert.AreEqual(persistedMessage.Body, message.Body);
        }

        [Test]
        public void DeleteAsync_DeleteNonExistingMessageThrows()
        {
            var (repo, _) = SetupTarget();

            const int nonExistingMessageId = 15;

            var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.DeleteAsync(nonExistingMessageId));

            Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.ResourceNotFound));
        }

        [Test]
        public void DeleteAsync_DeletingOtherUsersMessageIsNotAllowed()
        {
            var (repo, client) = SetupTarget();

            var otherUser = new User {Username = "User1", Password = "User1", Id = 1};

                
            client.SetupGet(x => x.CurrentUser).Returns(otherUser);
            var message = new Message {Header = "I am a header", Body = "I am a body"};
            repo.Context.Messages.Add(message);
            repo.Context.SaveChanges();

            var currentUser = new User {Username = "User2", Password = "User2", Id = 2};

            client.SetupGet(x => x.CurrentUser).Returns(currentUser);

            var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.DeleteAsync(message.Id));

            Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.NotAuthorized));
        }

        [Test]
        public async Task DeleteAsync_DeletedMessageIsPersisted()
        {
            var message = new Message {Header = "I am a header", Body = "I am a body", Id = 1};
            var (repo, _) = SetupTarget(message);

            await repo.DeleteAsync(message.Id);

            var deletedMessage = repo.Context.Messages.FirstOrDefault(x => x.Id == message.Id);
            Assert.IsNull(deletedMessage);
        }

        [Test]
        public void UpdateAsync_MissingHeaderThrows()
        {
            var message = new Message {Header = "I am a header", Body = "I am a body"};
            var (repo, _) = SetupTarget(message);

            message.Header = String.Empty;
            
            var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.UpdateAsync(message));

            Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.Required));

        }
        
        [Test]
        public void UpdateAsync_MissingBodyThrows()
        {
            var message = new Message {Header = "I am a header", Body = "I am a body" };
            var (repo, _) = SetupTarget(message);

            message.Body = String.Empty;
            
            var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.UpdateAsync(message));

            Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.Required));

        }
        
        [Test]
        public void UpdateAsync_NonExistingMessageThrows()
        {
            var (repo, _) = SetupTarget();

            const int nonExistantId = 15;
            var message = new Message {Header = "I am a header", Body = "I am a body", Id = nonExistantId };
            
            var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.UpdateAsync(message));

            Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.ResourceNotFound));

        }
        
        [Test]
        public void UpdateAsync_UpdatingOtherUsersMessageIsNotAllowed()
        {
            var (repo, client) = SetupTarget();

            var otherUser = new User {Username = "User1", Password = "User1", Id = 1};

                
            client.SetupGet(x => x.CurrentUser).Returns(otherUser);
            var message = new Message {Header = "I am a header", Body = "I am a body"};
            repo.Context.Messages.Add(message);
            repo.Context.SaveChanges();

            var currentUser = new User {Username = "User2", Password = "User2", Id = 2};

            client.SetupGet(x => x.CurrentUser).Returns(currentUser);

            var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.UpdateAsync(message));

            Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.NotAuthorized));
        }

        [Test]
        public async Task UpdateAsync_UpdatedMessageIsPersisted()
        {            
            var message = new Message {Header = "I am a header", Body = "I am a body", Id = 1 };
            var (repo, _) = SetupTarget(message);

            const string newBody = "I am a new body"; 
            
            message.Body = newBody;
            
            await repo.UpdateAsync(message);

            var updatedMessage = repo.Context.Messages.FirstOrDefault(x => x.Id == message.Id);

            Assert.IsNotNull(updatedMessage);
            Assert.AreEqual(updatedMessage.Body, newBody);
        }

        [Test]
        public async Task GetAsync_ReturnsAllMessages()
        {
            var messages = new List<Message>
            {
                new Message {Header = "header1", Body = "body1"},
                new Message {Header = "header2", Body = "body2"}
            };
      
            var (repo, _) = SetupTarget(messages.ToArray());

            var messagesInRepo = await repo.GetAsync();
            var messageList = messagesInRepo.ToList();
      
            Assert.IsTrue(messages.All(messageList.Contains));
            Assert.IsTrue(messageList.All(messages.Contains));
        }

        [Test]
        public void GetByIdAsync_NonExistingMessageThrows()
        {
            var (repo, _) = SetupTarget();

            const int nonExistantId = 15;
            
            var ex = Assert.ThrowsAsync<MessageStoreException>(async () => await repo.GetByIdAsync(nonExistantId));

            Assert.That(ex.ErrorCode, Is.EqualTo(ErrorCodes.ResourceNotFound));
        }
    }
}