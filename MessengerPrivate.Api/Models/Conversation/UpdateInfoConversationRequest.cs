namespace MessengerPrivate.Api.Models.Conversation
{
    public class UpdateInfoConversationRequest
    {
        public string Id { get; set; }

        public string? Name { get; set; }

        public IFormFile? AvatarUrlFile { get; set; }

        public string? LastMessageId { get; set; }
    }
}
