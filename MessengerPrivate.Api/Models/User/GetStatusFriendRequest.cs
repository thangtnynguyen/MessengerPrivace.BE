namespace MessengerPrivate.Api.Models.User
{
    public class GetStatusFriendRequest
    {
        public Guid UserId { get; set; }
        public Guid ContactId { get; set; }
    }
}
