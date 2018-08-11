using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasicMessageStore.Models;
using BasicMessageStore.Models.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicMessageStore.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class MessageController : RepositoryController<Message>
    {
        public MessageController(IMessageRepository messageRepository) : base(messageRepository)
        {
        }

        public override async Task<IActionResult> UpdateAsync(int id, Message model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var message = await Repository.GetByIdAsync(id);
            // Only update the body and header
            message.Body = model.Body;
            message.Header = model.Header;
            
            await Repository.UpdateAsync(message);
            return Ok();
        }
    }
}