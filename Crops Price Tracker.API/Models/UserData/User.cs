using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Crops_Price_Tracker.Models.UserData
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public string Mobile { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
