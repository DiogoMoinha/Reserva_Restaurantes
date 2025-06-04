using Reserva_Restaurantes.Areas.Identity.Pages.Account;
using Reserva_Restaurantes.Models.ApiModels;
using Reserva_Restaurantes.Services.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Reserva_Restaurantes.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenService _tokenService;

        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, TokenService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpGet]
        [Route("hello")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult Hello()
        {
            return Ok("Hello");
        }
        
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Authenticate(LoginApiModel loginRequest)
        {
            var identityUser = await _userManager.FindByEmailAsync(loginRequest.Email);
            if(identityUser == null)
                return BadRequest("Invalid user or password");
            
            var resultPassword = await _signInManager.CheckPasswordSignInAsync(identityUser, loginRequest.Password, 
                false);
            
            if(!resultPassword.Succeeded)
                return BadRequest("Invalid user or password");
            
            var token = _tokenService.GenerateToken(identityUser);
            
            return Ok(token);
        }
        
        [HttpPost]
        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            
            return NoContent();
        }
    }
}