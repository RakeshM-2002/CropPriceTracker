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

        public AlertController(AlertServices service)
        {
            _service = service;
        }

        /* ========= ADD ALERT ========= */
        [HttpPost("add")]
        public async Task<IActionResult> AddAlert([FromBody] AlertCreateDto dto)
        {
            if (string.IsNullOrEmpty(dto.UserId))
                return BadRequest("UserId is required");

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

        /* ========= GET ALERTS BY USER ========= */
        [HttpGet("my/{userId}")]
        public async Task<IActionResult> GetUserAlerts(string userId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var alerts = await _service.GetAlertsByUser(userId, startDate.UtcDateTime, endDate.UtcDateTime);
            return Ok(alerts);
        }
    }
}
