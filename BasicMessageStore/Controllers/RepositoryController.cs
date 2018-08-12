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
    /// <summary>
    /// Base controller for repository based controllers, implements the basic operations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Authorize]
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

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            await Repository.DeleteAsync(id);
            return Ok();
        }
    }
}