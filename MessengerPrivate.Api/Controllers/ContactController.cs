using AutoMapper;
using MessengerPrivate.Api.Data;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.Common;
using MessengerPrivate.Api.Models.Contact;
using MessengerPrivate.Api.Models.User;
using MessengerPrivate.Api.Services;
using MessengerPrivate.Api.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace MessengerPrivate.Api.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactController : ControllerBase
    {


        private readonly MongoDbContext _context;

        private readonly IHubContext<CallVideoHub> _chatHub;

        private readonly UserService _userService;

        private readonly IMapper _mapper;


        public ContactController(MongoDbContext context, IHubContext<CallVideoHub> chatHub, UserService userService, IMapper mapper)
        {
            _context = context;
            _chatHub = chatHub;
            _userService = userService;
            _mapper = mapper;

        }

        [HttpGet("get-by-user")]
        public async Task<ApiResult<ContactDto>> GetContactsByUserId([FromQuery] GetUserIdRequest request)
        {
            var contacts = await _context.Contacts.Find(contact => contact.UserId == request.UserId).ToListAsync();
            var contactDtos = _mapper.Map<ContactDto>(contacts);
            return new ApiResult<ContactDto>
            {
                Status = true,
                Message = "Successful",
                Data = contactDtos
            };
        }


        [HttpGet("status-friend")]
        public async Task<ApiResult<string>> GetStatusFriend([FromQuery] GetStatusFriendRequest request)
        {
            var filter = Builders<Contact>.Filter.And(
                Builders<Contact>.Filter.Eq(c => c.UserId, request.UserId),
                Builders<Contact>.Filter.Eq(c => c.ContactId, request.ContactId)
            );

            var contact = await _context.Contacts.Find(filter).FirstOrDefaultAsync();

            if (contact == null)
            {
                return new ApiResult<string>
                {
                    Status = false,
                    Message = "Fail",
                    Data = "None",
                };
            }

            return new ApiResult<string>
            {
                Status = true,
                Message = "Successful",
                Data = contact.Status,
            };


        }


        [HttpPost("send-friend-request")]
        public async Task<ApiResult<ContactDto>> SendFriendRequest([FromQuery] GetUserIdRequest request)
        {
            //Note request.UserId:receiveId
            var user = await _userService.GetUserInfo(HttpContext);

            var senderId = user.Id;
            var contact = new Contact
            {
                UserId = senderId,
                ContactId = request.UserId,
                Status = "pending"
            };

            await _context.Contacts.InsertOneAsync(contact);

            await _chatHub.Clients.User(request.UserId.ToString()).SendAsync("FriendRequestReceived", request.UserId.ToString(), senderId);

            var contactDto = _mapper.Map<ContactDto>(contact);
            return new ApiResult<ContactDto>
            {
                Status = true,
                Message = "Successful",
                Data = contactDto
            };
        }


        [HttpPut("accept-friend-request")]
        public async Task<ApiResult<bool>> AcceptFriendRequest([FromQuery] GetUserIdRequest request)
        {
            //Note request.UserId:senderId
            var user = await _userService.GetUserInfo(HttpContext);

            var receiverId = user.Id;
            var filter = Builders<Contact>.Filter.Where(c => c.UserId == request.UserId && c.ContactId == receiverId && c.Status == "pending");
            var update = Builders<Contact>.Update.Set(c => c.Status, "accepted");

            var result = await _context.Contacts.UpdateOneAsync(filter, update);

            if (result.MatchedCount > 0)
            {
                await _chatHub.Clients.User(receiverId.ToString()).SendAsync("FriendRequestAccepted", receiverId.ToString(), request.UserId.ToString());
            }

            return new ApiResult<bool>
            {
                Status = true,
                Message = "Successful",
                Data = true
            };

        }


        [HttpDelete("delete")]
        public async Task<ApiResult<bool>> DeleteContact([FromBody] DeleteRequest request)
        {
            var result = await _context.Contacts.DeleteOneAsync(contact => contact.Id == request.Id);
            if (result.DeletedCount == 0) return new ApiResult<bool>
            {
                Status = false,
                Message = "Not found",
                Data = false
            };
            return new ApiResult<bool>
            {
                Status = true,
                Message = "Successful",
                Data = true
            };
        }





    }
}
