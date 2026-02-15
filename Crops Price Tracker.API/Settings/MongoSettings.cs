namespace Crops_Price_Tracker.Settings
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CropCollection { get; set; }
        public string UserCollection { get; set; }
        public string AlertCollection { get; set; }
        public string OtpCollection { get; set; }
    }
}
