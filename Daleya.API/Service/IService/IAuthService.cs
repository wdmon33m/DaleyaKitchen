using Daleya.API.Models.Dto;
using Daleya.API.Models.Dto.Auth;
using Daleya.API.Utility;

namespace Daleya.API.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto> Register(RegistrationDto registrationRequestDto);
        Task<ResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<ResponseDto> AssignRole(string email, string? role = SD.RoleCustomer);
    }
}
