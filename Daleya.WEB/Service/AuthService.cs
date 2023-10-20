using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto.Auth;
using Daleya.WEB.Service.IService;
using Daleya.WEB.Utility;

namespace Daleya.WEB.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        private const string authApiUrl = "/api/auth/";

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = SD.DaleyaApiBase + authApiUrl + "login"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationDto registrationDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationDto,
                Url = SD.DaleyaApiBase + authApiUrl + "register"
            }, withBearer: false);
        }
    }
}
