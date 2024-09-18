namespace MessengerPrivate.Api.Models.Messenger
{
    public class UpdateMessengerRequest
    {
        public string Id { get; set; }

        public string? ConversationId { get; set; }

        public string? Content { get; set; }

        public string? Attachments { get; set; }

        public string? MessengerType { get; set; }

        public DateTime? SentTime { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
