using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MessengerPrivate.Api.Data.Entities.Interface;
using MessengerPrivate.Api.Models.Icon;

namespace MessengerPrivate.Api.Data.Entities
{
    public class Messenger: EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ConversationId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ConversationId { get; set; }

        [BsonElement("ReplyId")]
        public string ReplyId { get; set; } 

        [BsonElement("SenderId")]
        //[BsonRepresentation(BsonType.ObjectId)]
        public Guid SenderId { get; set; }

        [BsonElement("Content")]
        public string Content { get; set; } // Message content or file path

        [BsonElement("Icon")]
        public IconDto? Icon { get; set; }

        [BsonElement("Attachments")]
        public string Attachments { get; set; }

        [BsonElement("Medias")]
        public List<Media>? Medias { get; set; } // List of media objects

        [BsonElement("MessengerType")]
        public string MessengerType { get; set; } // text, image, video, file (1:text 2:image 3:video 4: file )

        [BsonElement("SentTime")]
        public DateTime SentTime { get; set; } = DateTime.Now;

        [BsonElement("Status")]
        public string Status { get; set; } // sent, received, read
    }

    public class Media
    {
        [BsonElement("Type")]
        public string Type { get; set; } // "image", "video", "audio",....

        [BsonElement("Url")]
        public string Url { get; set; } // The URL or path to the media

        [BsonElement("Name")]
        public string Name { get; set; } // The Name or path to the media

        [BsonElement("PathLocal")]
        public string? PathLocal { get; set; } // The URL or path to the media

    }

   
}
