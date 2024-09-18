using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MessengerPrivate.Api.Data.Entities.Interface;

namespace MessengerPrivate.Api.Data.Entities
{
    public class Contact: EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        //[BsonRepresentation(BsonType.ObjectId)]
        public Guid UserId { get; set; }

        [BsonElement("ContactId")]
        //[BsonRepresentation(BsonType.ObjectId)]
        public Guid ContactId { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; } // accepted, pending, blocked,none


    }
}
