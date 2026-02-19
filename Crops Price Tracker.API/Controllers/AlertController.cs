using Crops_Price_Tracker.Models.Alerts;
using Crops_Price_Tracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crops_Price_Tracker.Controllers
{
    [ApiController]
    [Route("api/alert")]
    public class AlertController : ControllerBase
    {
        private readonly AlertServices _service;
        private readonly ILogger<AlertController> _logger;

        public AlertController(AlertServices service, ILogger<AlertController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /* ========= ADD ALERT ========= */
        [HttpPost("add")]
        public async Task<IActionResult> AddAlert([FromBody] AlertCreateDto dto)
        {

            if (string.IsNullOrEmpty(dto.UserId))
                return BadRequest("UserId is required");

            try
            {

                var alert = new Alerts
                {
                    UserId = dto.UserId,
                    Crop = dto.Crop,
                    Market = dto.Market,
                    TargetPrice = dto.TargetPrice,
                    AboveBelow = dto.AboveBelow
                };

                await _service.AddAlert(alert);
                return Ok(new { message = "Alert added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding alert for user {UserId}", dto.UserId);
                return StatusCode(500, "An error occurred while adding the alert");
            }
        }

        /* ========= GET ALERTS BY USER ========= */
        [HttpGet("my/{userId}")]
        public async Task<IActionResult> GetUserAlerts(string userId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            try
            {
                var alerts = await _service.GetAlertsByUser(userId, startDate.UtcDateTime, endDate.UtcDateTime);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching alerts for user {UserId}", userId);
                return StatusCode(500, "An error occurred while fetching alerts");
            }
        }
    }
}
