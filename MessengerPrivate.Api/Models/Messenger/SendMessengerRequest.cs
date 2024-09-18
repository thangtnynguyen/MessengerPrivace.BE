using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MessengerPrivate.Api.Models.Messenger
{
    public class SendMessengerRequest
    {

        public string ConversationId { get; set; }

        public string? ReplyId { get; set; }

        public Guid? SenderId { get; set; }

        public string? Content { get; set; } 

        public string? Attachments { get; set; }

        public string? MessengerType { get; set; }

        public IconRequest? Icon { get; set; }

        public List<MediaFileRequest>? MediaFiles { get; set; }

        public DateTime? SentTime { get; set; } = DateTime.Now;

        public string? Status { get; set; } 


    }

    public class MediaFileRequest
    {
        public IFormFile File { get; set; }
        public string Type { get; set; } // "image", "video", "file"
        public string Name { get; set; }
    }

    public class IconRequest
    {
        public string? Url { get; set; }
        public string? Name { get; set; }
        public string? Data { get; set; }

    }
}
