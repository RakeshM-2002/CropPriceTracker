using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crops_Price_Tracker.Models.UserData

{
    public class OtpVerification
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Mobile { get; set; }
        public string Otp { get; set; }
        public DateTime Expiry { get; set; }
    }
}
