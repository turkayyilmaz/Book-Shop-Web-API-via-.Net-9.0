using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Presentations.ActionFilters;
using Services.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentations.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public AuthenticationController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpPost("register")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Register([FromBody] UserForRegistrationDto userForRegistrationDto)
        {
            var result = await _serviceManager.AuthenticationServices.RegisterUserAsync(userForRegistrationDto);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return StatusCode(201); // Created
        }
        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto userForAuthenticationDto)
        {
            var result = await _serviceManager.AuthenticationServices.ValidateUserAsync(userForAuthenticationDto);
            if (!result)
                return Unauthorized(); // 401
            var tokenDto = await _serviceManager.AuthenticationServices.CreateTokenAsync(populateExp:true);
            return Ok(tokenDto);
        }
        [HttpPost("refresh")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
        {
            //if (tokenDto is null) gerek yok çünkü validation filter zaten kontrol ediyor
            //    return BadRequest("Invalid client request");
            var newTokenDto = await _serviceManager.AuthenticationServices.RefreshTokenAsync(tokenDto);
            //if (newTokenDto is null) 
            //    return Unauthorized();
            return Ok(newTokenDto);
        }
    }
}
