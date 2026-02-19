using Crops_Price_Tracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crops_Price_Tracker.Controllers
{
    [ApiController]
    [Route("api/crop")]
    public class CropController : ControllerBase
    {
        private readonly CropService _service;

        public CropController(CropService service)
        {
            _service = service; 
        }

        [HttpGet("history")]
        public async Task<IActionResult> History(
    string cropName,
    DateTimeOffset startDate,
    DateTimeOffset endDate)
        {
            var result = await _service.History(
                cropName,
                startDate.UtcDateTime,
                endDate.UtcDateTime
            );
            return Ok(result);
        }

        [HttpGet("analyze")]
        public async Task<IActionResult> Analyze(
            string cropName,
            string marketName,
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            var result = await _service.Analyze(
                cropName,
                marketName,
                startDate.UtcDateTime,
                endDate.UtcDateTime
            );
            return Ok(result);
        }

        [HttpGet("cropName")]
        public async Task<IActionResult> GetCropNames(
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            var result = await _service.GetCropNames(startDate.UtcDateTime,
                endDate.UtcDateTime);
            return Ok(result);
        }

        [HttpGet("marketName")]
        public async Task<IActionResult> GetMarketNames(
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            var result = await _service.GetMarketNames(startDate.UtcDateTime,
                endDate.UtcDateTime);
            return Ok(result);
        }


    }
}
