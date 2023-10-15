using System.Net;

namespace Daleya.API.Models.Dto
{
    public class ResponseDto
    {
        public object? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public ResponseDto BadRequest(string? errorMessage = null)
        {
            StatusCode = HttpStatusCode.BadRequest;
            IsSuccess = false;
            ErrorMessage = errorMessage;
            return this;
        }

        public ResponseDto NoContent(string? errorMessage = null)
        {
            StatusCode = HttpStatusCode.NoContent;
            IsSuccess = false;
            ErrorMessage = errorMessage;
            return this;
        }

        public ResponseDto InternalServerError(string? errorMessage = null)
        {
            StatusCode = HttpStatusCode.InternalServerError;
            IsSuccess = false;
            ErrorMessage = errorMessage;
            return this;
        }

        public ResponseDto NotFound(string? errorMessage = null)
        {
            StatusCode = HttpStatusCode.NotFound;
            IsSuccess = false;
            ErrorMessage = errorMessage;
            return this;
        }
    }
}
