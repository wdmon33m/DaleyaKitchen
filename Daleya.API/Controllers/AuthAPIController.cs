using Daleya.API.Models.Dto;
using Daleya.API.Models.Dto.Auth;
using Daleya.API.Service.IService;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Daleya.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _response;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new();
        }

        [HttpPost("register")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDto>> Register([FromBody] RegistrationDto model)
        {
            if (model == null)
            {
                return _response.BadRequest("Please enter user name and passowrd");
            }

            var response = await _authService.Register(model);
            if (response == null || !response.IsSuccess)
            {
                return _response.BadRequest(response.ErrorMessage);
            }

            var assignRoleResponse = await _authService.AssignRole(model.Email);
            if (assignRoleResponse == null || !response.IsSuccess)
            {
                return _response.BadRequest(assignRoleResponse.ErrorMessage);
            }

            _response.Result = "User has been registerd successfully";
            _response.StatusCode = HttpStatusCode.Created;
            return _response;
        }


        [HttpPost("login")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDto>> Login([FromBody] LoginRequestDto model)
        {
             return await _authService.Login(model);
        }
    }
}
