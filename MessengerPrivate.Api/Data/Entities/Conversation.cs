using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MessengerPrivate.Api.Data.Entities.Interface;

namespace MessengerPrivate.Api.Data.Entities
{
    public class Conversation: EntityBase
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; } // personal or group

        [BsonElement("Participants")]
        public List<Guid> Participants { get; set; } // List of User Ids

        [BsonElement("Administrators")]
        public List<Guid>? Administrators { get; set; } // List of Administrator User Ids

        [BsonElement("Name")]
        public string? Name { get; set; }

        [BsonElement("AvatarUrl")]
        public string? AvatarUrl { get; set; }

        [BsonElement("Censor")]
        public bool? Censor { get; set; } // approve a new member join group

        [BsonElement("LastMessageId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? LastMessageId { get; set; }



    }
}
