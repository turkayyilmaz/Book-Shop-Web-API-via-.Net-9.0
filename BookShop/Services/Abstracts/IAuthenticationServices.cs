using Entities.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstracts
{
    public interface IAuthenticationServices
    {
        Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistrationDto);
        Task<bool> ValidateUserAsync(UserForAuthenticationDto userForAuthenticationDto);
        Task<TokenDto> CreateTokenAsync(bool populateExp);
        Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto);
    }
}
