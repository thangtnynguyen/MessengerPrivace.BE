using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MessengerPrivate.Api.Data.Entities.Interface;

namespace MessengerPrivate.Api.Data.Entities
{
    public class Icon:EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        [BsonElement("Type")]
        public string? Type { get; set; }//image, svg,....

        [BsonElement("Url")]
        public string? Url { get; set; } 

        [BsonElement("Data")]
        public string? Data { get; set; }

        [BsonElement("Name")]
        public string? Name { get; set; } 


    }
}
