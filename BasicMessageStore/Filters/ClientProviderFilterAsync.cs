using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BasicMessageStore.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Remotion.Linq.Clauses.ResultOperators;

namespace BasicMessageStore.Security
{
    /// <summary>
    /// Filter to assign current user before anything else is executed
    /// TODO: Replace with built in IdentityUser and IdentityRole
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ClientProviderFilterAsync : Attribute, IAsyncActionFilter
    {
        private readonly IClientProvider _clientProvider;
        private readonly IUserRepository _userRepository;

        public ClientProviderFilterAsync(IClientProvider clientProvider, IUserRepository userRepository)
        {
            _clientProvider = clientProvider;
            _userRepository = userRepository;
        }
        
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!(context.Controller is Controller controller))
                return;

            var userClaim = controller.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name);

            var username = userClaim?.Value;

            if (username != null)
            {
                var user = await _userRepository.GetByUsername(username);

                if (user != null)
                {
                    _clientProvider.CurrentUser = user;    
                }
            }
            await next();
        }
    }
}