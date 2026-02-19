using System.ComponentModel.DataAnnotations;

namespace Crops_Price_Tracker.Models.Crop
{
    public class HistoryRequest
    {
        [Required]
        public string CropName { get; set; }
    }
}
