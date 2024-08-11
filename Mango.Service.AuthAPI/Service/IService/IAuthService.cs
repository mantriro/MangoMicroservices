using Mango.Service.AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Service.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);

        Task<bool> AssignRole(string email, string roleName);

    }
}
