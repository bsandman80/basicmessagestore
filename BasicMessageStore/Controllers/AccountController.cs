using System.Collections.Generic;
using System.Threading.Tasks;
using BasicMessageStore.Models;
using BasicMessageStore.Models.Security;
using BasicMessageStore.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicMessageStore.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : RepositoryController<User>
    {

        public AccountController(IUserRepository userRepository) : base(userRepository)
        {
        }

        [Authorize]
        public override async Task<IActionResult> Delete(int id)
        {
            return await base.Delete(id);
        }

        [Authorize]
        public override async Task<IActionResult> UpdateAsync(int id, User model)
        {
            return await base.UpdateAsync(id, model);
        }

        [Authorize]
        public override async Task<IActionResult> GetAsync()
        {
            return await base.GetAsync();
        }
    }
}