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
    public class MessageController : RepositoryController<Message>
    {
        public MessageController(IMessageRepository messageRepository) : base(messageRepository)
        {
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, string header, string body)
        {
            var message = await Repository.GetByIdAsync(id);
            message.Header = header;
            message.Body = body;
            
            await Repository.UpdateAsync(message);
            return Ok();
        }
        
        [HttpPost]
        public  async Task<IActionResult> AddAsync(string header, string body)
        {
            var message = new Message {Header = header, Body = body};
            message = await Repository.AddAsync(message);
            return CreatedAtAction(nameof(GetAsync), new {id = message.Id}, message);
        }
    }
}