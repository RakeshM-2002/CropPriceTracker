using System.ComponentModel.DataAnnotations;

namespace Crops_Price_Tracker.Models.Crop
{
    public class AnalyzeRequest
    {
        [Required]
        public string CropName { get; set; }

        [Required]
        public string MarketName { get; set; }
    }
}
