using MessengerPrivate.Api.Models.Common;

namespace MessengerPrivate.Api.Models.Messenger
{
    public class GetMessengerByConversationIdRequest:PagingRequest
    {
        public string ConversationId {  get; set; }

    }
}
