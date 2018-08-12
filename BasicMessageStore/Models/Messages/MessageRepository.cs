using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BasicMessageStore.Exceptions;
using BasicMessageStore.Security;
using Microsoft.EntityFrameworkCore;

namespace BasicMessageStore.Models.Messages
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IClientProvider _clientProvider;
        public MessageStoreContext Context { get; set; }

        public MessageRepository(MessageStoreContext context, IClientProvider clientProvider)
        {
            Context = context;
            _clientProvider = clientProvider;   
        }
        
        public async Task<Message> AddAsync(Message message)
        {
            // TODO: Implement attributes for validation to avoid these kind of checks
            if (String.IsNullOrWhiteSpace(message.Header))
                throw new MessageStoreException(ErrorCodes.Required, "Header is required");
            if (String.IsNullOrWhiteSpace(message.Body))
                throw new MessageStoreException(ErrorCodes.Required, "Body is required");
            Context.Messages.Add(message);
            await Context.SaveChangesAsync();
            return message;
        }

        public async Task DeleteAsync(int id)
        {
            var message = await GetByIdAsync(id);
            if (message == null)
                throw new MessageStoreException(ErrorCodes.ResourceNotFound, "Message could not be found");
            
            if (_clientProvider.CurrentUser?.Id != message.CreatedBy?.Id)
                throw new MessageStoreException(ErrorCodes.NotAuthorized, "User not authorized");            
            
            Context.Messages.Remove(message);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Message message)
        {
            if (String.IsNullOrWhiteSpace(message.Header))
                throw new MessageStoreException(ErrorCodes.Required, "Header is required");
            if (String.IsNullOrWhiteSpace(message.Body))
                throw new MessageStoreException(ErrorCodes.Required, "Body is required");
            
            // Make sure message actually exists
            var existingMessage  = await GetByIdAsync(message.Id);
            
            if (existingMessage == null)
                throw new MessageStoreException(ErrorCodes.ResourceNotFound, "Message could not be found");
            
            if (_clientProvider.CurrentUser?.Id != message.CreatedBy?.Id)
                throw new MessageStoreException(ErrorCodes.NotAuthorized, "User not authorized");            
            
            Context.Messages.Update(message);
            await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetAsync()
        {
            return await Context.Messages.ToListAsync();
        }

        public async Task<Message> GetByIdAsync(int id)
        {
            var message = await Context.Messages.FindAsync(id);
            if (message == null)
                throw new MessageStoreException(ErrorCodes.ResourceNotFound, "Message could not be found");
            return message;
        }
    }
}