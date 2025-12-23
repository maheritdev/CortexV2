using Cortex.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Cortex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        protected IActionResult ApiResult<T>(BaseResponse<T> response)
        {
            return response.Success ? Ok(response) : BadRequest(response);
        }

        protected IActionResult ApiCreated<T>(T data, string message = "Created successfully")
        {
            var response = BaseResponse<T>.Created(data, message);
            return Created("", response);
        }

        protected IActionResult ApiOk<T>(T data, string message = "Success")
        {
            var response = BaseResponse<T>.Ok(data, message);
            return Ok(response);
        }

        protected IActionResult ApiNotFound<T>(string message = "Not found")
        {
            var response = BaseResponse<T>.NotFound(message);
            return NotFound(response);
        }

        protected IActionResult ApiValidationError<T>(List<string> errors, string message = "Validation failed")
        {
            var response = BaseResponse<T>.ValidationError(errors, message);
            return BadRequest(response);
        }

        protected IActionResult ApiFail<T>(string message, List<string>? errors = null)
        {
            var response = BaseResponse<T>.Fail(message, errors);
            return BadRequest(response);
        }
    }
}