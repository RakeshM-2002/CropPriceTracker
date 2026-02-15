namespace Crops_Price_Tracker.Models.Crop
{
    public class CropHistoryDto
    {
        public string Commodity { get; set; }
        public string Market { get; set; }
        public decimal ModalPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
