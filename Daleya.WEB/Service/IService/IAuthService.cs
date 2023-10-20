using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto.Auth;

namespace Daleya.WEB.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto?> RegisterAsync(RegistrationDto registrationDto);
    }
}
