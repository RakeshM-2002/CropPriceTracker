namespace Crops_Price_Tracker.Models.Alerts
{
    public class AlertCreateDto
    {
        public string UserId { get; set; }

        public string Crop { get; set; }

        public string Market { get; set; }

        public decimal TargetPrice { get; set; }

        public string AboveBelow { get; set; } 
    }
}
