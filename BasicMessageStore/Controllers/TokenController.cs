using System.Threading.Tasks;
using BasicMessageStore.Models.Users;
using BasicMessageStore.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicMessageStore.Controllers
{
    
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenProvider _tokenProvider;

        public TokenController(IUserRepository userRepository, ITokenProvider tokenProvider)
        {
            _userRepository = userRepository;
            _tokenProvider = tokenProvider;
        }
        
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAsync(string username, string password)
        {
            if (!await _userRepository.Login(username, password))
                return BadRequest("Invalid username or password");

            var user = await _userRepository.GetByUsername(username);

            return Ok(new
            {
                AccessToken = _tokenProvider.GenerateAccessToken(user)
            });
        }
    }
}