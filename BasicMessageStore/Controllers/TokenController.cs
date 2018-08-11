using System.Threading.Tasks;
using BasicMessageStore.Models.Security;
using BasicMessageStore.Security;
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
        public async Task<IActionResult> CreateAsync([FromForm]string username, [FromForm]string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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