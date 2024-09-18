using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.Icon;

namespace MessengerPrivate.Api.Models.Messenger
{
    public class MessengerDto
    {

        public string Id { get; set; }

        public string ConversationId { get; set; }

        public string ReplyId { get; set; }

        public Guid SenderId { get; set; }

        public string Content { get; set; }

        public string Attachments { get; set; }

        public IconDto? Icon { get; set; }

        public List<Media>? Medias { get; set; } 

        public string MessengerType { get; set; } 

        public DateTime SentTime { get; set; } 

        public string Status { get; set; } 

        public DateTime CreatedAt {  get; set; }
    }
}
