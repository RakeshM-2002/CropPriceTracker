using System.ComponentModel.DataAnnotations;

namespace Crops_Price_Tracker.Models.UserData
{
    public class GenerateOtpRequest
    {

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[6-9]\d{9}$")]
        public string Mobile { get; set; }
    }
}
