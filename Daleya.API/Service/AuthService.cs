using Daleya.API.Models.Dto.Auth;
using Daleya.API.Models;
using Microsoft.AspNetCore.Identity;
using Daleya.API.Models.Dto;
using Daleya.API.Service.IService;
using System.Net;
using Daleya.API.Utility;

namespace Daleya.API.Service
{
    public class AuthService : IAuthService
    { 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        protected ResponseDto _response;

        public AuthService(UserManager<ApplicationUser> userManager, 
            IJwtTokenGenerator jwtTokenGenerator, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _response = new();
            _roleManager = roleManager;
        }

        public async Task<ResponseDto> AssignRole(string email, string? role = SD.RoleCustomer)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email.ToLower());
                if (user == null)
                {
                    return _response.BadRequest("User is not exist!");
                }
                if (!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                {
                    return _response.BadRequest("Role is not exist!");
                }

                var result = await _userManager.AddToRoleAsync(user, role);

                if (result == null || !result.Succeeded)
                {
                    return _response.BadRequest(result?.Errors.First().Description);
                }

                _response.Result = "Role : " + role + " added to " + email + " Successufully";
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        public async Task<ResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginRequestDto.UserName.ToLower());

                bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

                if (user == null || !isPasswordValid)
                {
                    return _response.BadRequest("Username or password is incorrect");
                }

                //if user was found, Generate GWT Token

                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtTokenGenerator.GenerateToken(user, roles);

                UserDto userDto = new()
                {
                    Email = user.Email,
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                };

                LoginResponseDto loginResponseDto = new LoginResponseDto()
                {
                    User = userDto,
                    Token = token
                };
                _response.Result = loginResponseDto;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        public async Task<ResponseDto> Register(RegistrationDto registrationRequestDto)
        {
            try
            {

                ApplicationUser user = new()
                {
                    UserName = registrationRequestDto.Email,
                    Email = registrationRequestDto.Email,
                    NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                    FirstName = registrationRequestDto.FirstName,
                    LastName = registrationRequestDto.LastName,
                    PhoneNumber = registrationRequestDto.PhoneNumber,
                    CreatedDate = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result == null || !result.Succeeded)
                {
                    return _response.BadRequest(result.Errors.First().Description);
                }
                user.Id = _userManager.FindByNameAsync(user.UserName.ToLower()).GetAwaiter().GetResult().Id;

                UserDto userDto = new()
                {
                    Email = user.Email,
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                };

                _response.StatusCode = HttpStatusCode.Created;
                _response.Result = userDto;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }
    }
}
