namespace Crops_Price_Tracker.Models.Alerts
{
    public class AlertResponseDto
    {
        public string Crop { get; set; }
        public string Market { get; set; }
        public decimal LatestPrice { get; set; }
        public decimal TargetPrice { get; set; }
        public string Condition { get; set; }   // Above / Below
        public DateTime CreatedAt { get; set; }

        // UI formatted message
        public string Message { get; set; }
    }
}
