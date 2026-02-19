using Crops_Price_Tracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crops_Price_Tracker.Controllers
{
    [ApiController]
    [Route("api/crop")]
    public class CropController : ControllerBase
    {
        private readonly CropService _service;
        private readonly ILogger<CropController> _logger;

        public CropController(CropService service, ILogger<CropController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("history")]
        public async Task<IActionResult> History(
    string cropName,
    DateTimeOffset startDate,
    DateTimeOffset endDate)
        {
            if (string.IsNullOrEmpty(cropName))
                return BadRequest("Crop name is required");

            try
            {
                var result = await _service.History(
                    cropName,
                    startDate.UtcDateTime,
                    endDate.UtcDateTime
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching history for crop {CropName}", cropName);
                return StatusCode(500, "Internal server error. Please try again.");
            }
        }

        [HttpGet("analyze")]
        public async Task<IActionResult> Analyze(
            string cropName,
            string marketName,
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            try
            {
                var result = await _service.Analyze(
                    cropName,
                    marketName,
                    startDate.UtcDateTime,
                    endDate.UtcDateTime
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing crop {CropName} in market {MarketName}", cropName, marketName);
                return StatusCode(500, "Internal server error. Please try again.");
            }
        }

        [HttpGet("cropName")]
        public async Task<IActionResult> GetCropNames(
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {

            try
            {
                var result = await _service.GetCropNames(startDate.UtcDateTime,
                    endDate.UtcDateTime);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching crop names between {StartDate} and {EndDate}", startDate, endDate);
                return StatusCode(500, "Internal server error. Please try again.");
            }
        }

        [HttpGet("marketName")]
        public async Task<IActionResult> GetMarketNames(
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            try
            {
                var result = await _service.GetMarketNames(startDate.UtcDateTime,
                    endDate.UtcDateTime);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching market names between {StartDate} and {EndDate}", startDate, endDate);
                return StatusCode(500, "Internal server error. Please try again.");
            }
        }

    }
}
