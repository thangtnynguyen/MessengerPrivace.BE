namespace MessengerPrivate.Api.Models.Messenger
{
    public class UpdateMessengerStatusRequest
    {

        public string Id { get; set; }

        public string? ConversationId { get; set; }

        public string? Status { get; set; } 

        public DateTime? UpdatedAt { get; set; }


    }
}
