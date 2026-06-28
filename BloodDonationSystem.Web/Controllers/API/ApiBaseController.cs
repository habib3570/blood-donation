using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.API
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public abstract class ApiBaseController : ControllerBase
    {
        protected IActionResult ApiResponse<T>(Application.Common.Models.Result<T> result)
        {
            if (result.IsSuccess)
                return Ok(new { success = true, data = result.Data, message = result.Message });
            return BadRequest(new { success = false, errors = result.Errors });
        }

        protected IActionResult ApiResponse(Application.Common.Models.Result result)
        {
            if (result.IsSuccess)
                return Ok(new { success = true, message = result.Message });
            return BadRequest(new { success = false, errors = result.Errors });
        }
    }
}