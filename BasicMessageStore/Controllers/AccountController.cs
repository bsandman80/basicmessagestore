using System.Threading.Tasks;
using BasicMessageStore.Models.Users;
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

        [HttpPost]
        [AllowAnonymous]
        public  async Task<IActionResult> AddAsync([FromBody]User user)
        {
            user = new User {Username = user.Username, Password = user.Password};
            user = await Repository.AddAsync(user);
            return CreatedAtAction(nameof(GetAsync), new {id = user.Id}, user);
        }
    }
}