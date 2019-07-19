using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NG_Core_Auth.Helpers;
using NG_Core_Auth.JWT;
using NG_Core_Auth.Models.Entities;
using NG_Core_Auth.ViewModels;

namespace NG_Core_Auth.Controllers
{
    
    [Route("api/[controller]")]
    public class AuthController: Controller
    {private readonly UserManager<AppUser> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;

        public AuthController(UserManager<AppUser> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody]CredentialsViewModel credentials){
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var identity = await GetClaimsIdentity(credentials.UserName,credentials.Password);
            if (identity == null)
                return BadRequest(Errors.AddErrorToModelState("login_failure","Invalid user or password",ModelState));

            var jwt = await Tokens.GenerateJwt(identity,_jwtFactory,credentials.UserName,_jwtOptions,new JsonSerializerSettings { Formatting = Formatting.Indented });

            return new OkObjectResult(jwt);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string username,string password){
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);
            
            var userToVerify = await _userManager.FindByNameAsync(username);

            if (userToVerify == null)
                return await Task.FromResult<ClaimsIdentity>(null);
            
            if (await _userManager.CheckPasswordAsync(userToVerify,password))
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(username,userToVerify.Id.ToString()));

            return await Task.FromResult<ClaimsIdentity>(null);

        }
        
    }
}