using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BasicMessageStore.Models.Messages
{
    public class MessageRepository : IMessageRepository
    {
        public MessageStoreContext Context { get; set; }

        public MessageRepository(MessageStoreContext context)
        {
            Context = context;
        }
        
        
        public async Task<Message> AddAsync(Message message)
        {
            Context.Messages.Add(message);
            await Context.SaveChangesAsync();
            return message;
        }

        public async Task DeleteAsync(int id)
        {
            var message = await GetByIdAsync(id);            
            Context.Messages.Remove(message);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Message message)
        {
            Context.Messages.Update(message);
            await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetAsync()
        {
            return await Context.Messages.ToListAsync();
        }

        public async Task<Message> GetByIdAsync(int id)
        {
            return await Context.Messages.FindAsync(id);
        }
    }
}