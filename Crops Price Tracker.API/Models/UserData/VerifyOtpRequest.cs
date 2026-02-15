using System.ComponentModel.DataAnnotations;

namespace Crops_Price_Tracker.Models.UserData
{
    public class VerifyOtpRequest
    {
        [Required]
        public string Mobile { get; set; }

        [Required]
        public string Otp { get; set; }
    }
}
