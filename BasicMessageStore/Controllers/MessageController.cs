using System.Threading.Tasks;
using BasicMessageStore.Models.Messages;
using Microsoft.AspNetCore.Mvc;

namespace BasicMessageStore.Controllers
{
    [Route("api/[controller]")]
    public class MessageController : RepositoryController<Message>
    {
        public MessageController(IMessageRepository messageRepository) : base(messageRepository)
        {
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody]Message message)
        {            
            var existingMessage = await Repository.GetByIdAsync(id);
            existingMessage.Header = message.Header;
            existingMessage.Body = message.Body;
            
            await Repository.UpdateAsync(message);
            return Ok();
        }
        
        [HttpPost]
        public  async Task<IActionResult> AddAsync([FromBody]Message message)
        {
            message = new Message {Header = message.Header, Body = message.Body};
            message = await Repository.AddAsync(message);
            return CreatedAtAction(nameof(GetAsync), new {id = message.Id}, message);
        }
    }
}