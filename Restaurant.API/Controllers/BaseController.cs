using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Common;

namespace Restaurant.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "Success", int statusCode = 200)
        {
            return StatusCode(statusCode, ApiResponseDto<T>.SuccessResponse(data, message));
        }

        protected IActionResult Created<T>(T data, string message = "Created successfully")
        {
            return StatusCode(210, ApiResponseDto<T>.SuccessResponse(data, message));
        }

        protected IActionResult Error(string message, List<string>? errors = null, int statusCode = 400)
        {
            return StatusCode(statusCode, ApiResponseDto<object>.ErrorResponse(message, errors));
        }

        protected IActionResult ResultResponse<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return Success(result.Value, result.Message ?? "Success");

            return Error(result.Message ?? "Operation failed", result.Errors);
        }
    }
}
