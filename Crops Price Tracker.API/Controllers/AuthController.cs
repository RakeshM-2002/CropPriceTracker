using Crops_Price_Tracker.Models.UserData;
using Crops_Price_Tracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crops_Price_Tracker.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _service;

        public AuthController(AuthService service)
        {
            _service = service;
        }

        [HttpPost("generate-otp")]
        public async Task<IActionResult> Generate([FromBody] GenerateOtpRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.GenerateOtp(request.Mobile);
            return Ok("OTP Sent");
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> Verify([FromBody] VerifyOtpRequest request)
        {
            var user = await _service.VerifyOtp(request.Mobile, request.Otp).ConfigureAwait(false);

            if (user == null)
                return BadRequest("Invalid or Expired OTP");

            //var token = _service.GenerateJwt(user);

            return Ok(new
            {
                userId = user.UserId,
                mobile = user.Mobile,
                //token = token
            });
        }

    }
}
