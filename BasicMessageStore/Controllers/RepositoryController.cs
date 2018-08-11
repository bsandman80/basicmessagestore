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
    public abstract class RepositoryController<T> : Controller where T : Model
    {
        protected readonly IRepository<T> Repository;

        protected RepositoryController(IRepository<T> repository)
        {
            Repository = repository;
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAsync()
        {
            var models = await Repository.GetAsync();
            return Ok(models);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetAsync(int id)
        {
            return Ok(await Repository.GetByIdAsync(id));
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddAsync([FromForm]T model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var created = await Repository.AddAsync(model);
            return CreatedAtAction("GetAsync", new {id = created.Id}, created);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> UpdateAsync(int id, [FromForm]T model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.Id = id;
            await Repository.UpdateAsync(model);
            return Ok();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            await Repository.DeleteAsync(id);
            return Ok();
        }
    }
}