using System;
using BasicMessageStore.Models;
using BasicMessageStore.Security;
using Microsoft.EntityFrameworkCore;

namespace BasicMessageStore.Test.Models
{
    public class RepositoryTestCommon
    {
        public MessageStoreContext GetContext(IClientProvider clientProvider)
        {
            // Mocking entity framework seems like a bad idea. Use the provided in memory database for tests
            var options = new DbContextOptionsBuilder<MessageStoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return  new MessageStoreContext(options, clientProvider);
        }
    }
}