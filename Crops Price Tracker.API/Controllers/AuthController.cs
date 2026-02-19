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
        private readonly ILogger<AuthController> _logger;


        public AuthController(AuthService service, ILogger<AuthController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("generate-otp")]
        public async Task<IActionResult> Generate([FromBody] GenerateOtpRequest request)
        {
           
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

            try { 
                await _service.GenerateOtp(request.Mobile);
                return Ok("OTP Sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Generate OTP failed for mobile {Mobile}",
                    request.Mobile);

                return StatusCode(500,
                    "Internal server error. Please try again.");
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> Verify([FromBody] VerifyOtpRequest request)
        {

            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "OTP verification failed for mobile {Mobile}",
                    request.Mobile);
                return StatusCode(500,
                    "Internal server error. Please try again.");
            }
        }

        }
    } 
