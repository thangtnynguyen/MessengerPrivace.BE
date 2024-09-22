using Amazon.Runtime.Internal;
using AutoMapper;
using MessengerPrivate.Api.Constants;
using MessengerPrivate.Api.Data;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.Common;
using MessengerPrivate.Api.Models.Conversation;
using MessengerPrivate.Api.Models.Messenger;
using MessengerPrivate.Api.Models.User;
using MessengerPrivate.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace MessengerPrivate.Api.Controllers
{
    [Route("api/conversation")]
    [ApiController]
    public class ConversationController : ControllerBase
    {

        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;
        private readonly FileService _fileService;


        public ConversationController(MongoDbContext context, IMapper mapper, UserService userService, FileService fileService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _fileService = fileService;
            _fileService = fileService;
        }

        [HttpGet("get-by-user")]
        public async Task<ApiResult<PagingResult<ConversationDto>>> GetConversationsByUserId([FromQuery] GetConversationByUserRequest request)
        {

            var user = await _userService.GetUserInfo(HttpContext);
            if (user == null)
            {
                return new ApiResult<PagingResult<ConversationDto>>
                {
                    Status = false,
                    Message = "User not found",
                    Data = null
                };
            }

            var filter = Builders<Conversation>.Filter.And(
            Builders<Conversation>.Filter.AnyEq(conversation => conversation.Participants, user.Id),
            Builders<Conversation>.Filter.Exists(conversation => conversation.LastMessageId, true) // Chỉ lấy những cuộc trò chuyện có tin nhắn cuối
            );

            // Lấy tất cả các cuộc trò chuyện thỏa mãn điều kiện
            var conversations = await _context.Conversations
                .Find(filter)
                .ToListAsync();

            // Lấy tất cả các cuộc trò chuyện của người dùng
            //var conversations = await _context.Conversations
            //    .Find(conversation => conversation.Participants.Contains(user.Id))
            //    .ToListAsync();

            // Danh sách ConversationDto
            var conversationDtos = new List<ConversationDto>();

            foreach (var conversation in conversations)
            {
                // Lấy tin nhắn cuối cùng của mỗi cuộc trò chuyện
                var lastMessage = await _context.Messengers
                    .Find(m => m.ConversationId == conversation.Id)
                    .SortByDescending(m => m.SentTime)
                    .FirstOrDefaultAsync();

                // Sử dụng AutoMapper để chuyển đổi Conversation sang ConversationDto
                var conversationDto = _mapper.Map<ConversationDto>(conversation);

                // Gán nội dung tin nhắn cuối cùng vào ConversationDto
                var lastMessageDto = _mapper.Map<MessengerDto>(lastMessage);
                conversationDto.Messenger = lastMessageDto;

                // Kiểm tra Type và AvatarUrl của cuộc trò chuyện
                if (conversation.Type == "group" && string.IsNullOrEmpty(conversationDto.AvatarUrl))
                {
                    // Lấy danh sách 3 người đầu tiên trong Participants nếu là group và AvatarUrl == null
                    var firstThreeParticipantsIds = conversation.Participants.Take(3).ToList();

                    // Truy vấn thông tin của 3 người đầu tiên
                    var firstThreeParticipants = await _context.Users
                        .Find(user => firstThreeParticipantsIds.Contains(user.Id))
                        .ToListAsync();

                    // Gán danh sách người dùng vào DTO
                    conversationDto.ParticipantInfos = firstThreeParticipants
                        .Select(u => new UserDto
                        {
                            Id = u.Id,
                            Name = u.Name,
                            AvatarUrl = u.AvatarUrl
                        }).ToList();
                }
                else
                {
                    foreach(var parti in conversation.Participants)
                    {
                        if (parti != user.Id)
                        {
                            var participants = await _context.Users
                                                    .Find(user => parti==user.Id)
                                                    .ToListAsync();

                            // Gán danh sách người dùng vào DTO
                            conversationDto.ParticipantInfos = participants
                                .Select(u => new UserDto
                                {
                                    Id = u.Id,
                                    Name = u.Name,
                                    AvatarUrl = u.AvatarUrl
                                }).ToList();

                        }
                    }
                }

                conversationDtos.Add(conversationDto);
            }
            if (conversationDtos.Count==0) {
                return new ApiResult<PagingResult<ConversationDto>>
                {
                    Status = false,
                    Message = "Not found",
                    Data = null
                };
            }
            // Sắp xếp danh sách ConversationDto theo thời gian của tin nhắn cuối cùng
            var sortedConversations = conversationDtos
                .Where(c => c.Messenger != null)
                .OrderByDescending(c => c.Messenger.SentTime)
                .ToList();

            // Áp dụng phân trang
            var totalRecords = sortedConversations.Count;
            var pagedConversations = sortedConversations
                .Skip(((request.PageIndex ?? 1) - 1) * (request.PageSize ?? 10))
                .Take(request.PageSize ?? 10)
                .ToList();

            // Tạo kết quả phân trang
            var pagingResult = new PagingResult<ConversationDto>(pagedConversations, request.PageIndex ?? 1, request.PageSize ?? 10, totalRecords);

            return new ApiResult<PagingResult<ConversationDto>>
            {
                Status = true,
                Message = "Successful",
                Data = pagingResult
            };
        }



        [HttpGet("get-by-id")]
        public async Task<ApiResult<ConversationDto>> GetConversationById([ FromQuery]EntityIdentityRequest<string> request)
        {

            var currentUser = await _userService.GetUserInfo(HttpContext);

            // Kiểm tra ID có hợp lệ hay không
            if (string.IsNullOrEmpty(request.Id))
            {
                return new ApiResult<ConversationDto>
                {
                    Status = false,
                    Message = "Invalid conversation ID.",
                    Data = null
                };
            }

            // Tìm cuộc trò chuyện theo ID
            var conversation = await _context.Conversations.Find(c => c.Id == request.Id).FirstOrDefaultAsync();

            if (conversation == null)
            {
                return new ApiResult<ConversationDto>
                {
                    Status = false,
                    Message = "Conversation not found.",
                    Data = null
                };
            }

            // Chuyển đổi sang DTO
            var conversationDto = _mapper.Map<ConversationDto>(conversation);

            // Kiểm tra Type và AvatarUrl của cuộc trò chuyện
            if (conversation.Type == "group" && string.IsNullOrEmpty(conversationDto.AvatarUrl))
            {
                // Lấy danh sách 3 người đầu tiên trong Participants nếu là group và AvatarUrl == null
                var firstThreeParticipantsIds = conversation.Participants.Take(3).ToList();

                // Truy vấn thông tin của 3 người đầu tiên
                var firstThreeParticipants = await _context.Users
                    .Find(user => firstThreeParticipantsIds.Contains(user.Id))
                    .ToListAsync();

                // Gán danh sách người dùng vào DTO
                conversationDto.ParticipantInfos = firstThreeParticipants
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        AvatarUrl = u.AvatarUrl
                    }).ToList();
            }
            else
            {
                foreach (var parti in conversation.Participants)
                {
                    if (parti != currentUser.Id)
                    {
                        var participants = await _context.Users
                                                .Find(user => parti == user.Id)
                                                .ToListAsync();

                        // Gán danh sách người dùng vào DTO
                        conversationDto.ParticipantInfos = participants
                            .Select(u => new UserDto
                            {
                                Id = u.Id,
                                Name = u.Name,
                                AvatarUrl = u.AvatarUrl
                            }).ToList();

                    }
                }
            }

            // Nếu cần, lấy thông tin người tham gia khác
            var currentUserId = currentUser.Id; // Giả sử currentUser có thuộc tính Id kiểu Guid
            var otherUserId = conversation.Participants.FirstOrDefault(id => id != currentUserId);

            if (otherUserId != null)
            {
                var otherUser = await _userService.GetUserById(otherUserId);
                conversationDto.Name = otherUser.Name; // Gán tên của người kia cho cuộc trò chuyện
            }

            return new ApiResult<ConversationDto>
            {
                Status = true,
                Message = "Successful",
                Data = conversationDto
            };
        }





        [HttpPost("create-or-get")]
        public async Task<ApiResult<ConversationDto>> CreateConversation([FromBody] CreateConversationRequest request)
        {

            // Kiểm tra nếu kiểu Type là "personal"
            if (request.Type == "personal")
            {
                // Lấy thông tin người dùng hiện tại từ HttpContext
                var currentUser = await _userService.GetUserInfo(HttpContext);
                var currentUserId = currentUser.Id; // Giả sử currentUser có thuộc tính Id kiểu Guid

                // Đảm bảo rằng chỉ có 2 phần tử trong mảng Participants
                if (request.Participants.Count != 2)
                {
                    return new ApiResult<ConversationDto>
                    {
                        Status = false,
                        Message = "A personal conversation must have exactly two participants.",
                        Data = null
                    };
                }

                // Kiểm tra xem đã tồn tại cuộc trò chuyện personal nào giữa hai người dùng này chưa
                var existingConversation = await _context.Conversations
                    .Find(conversation =>
                        conversation.Type == "personal" &&
                        conversation.Participants.Contains(request.Participants[0]) &&
                        conversation.Participants.Contains(request.Participants[1]) &&
                        conversation.Participants.Count == 2) // Đảm bảo rằng cuộc trò chuyện chỉ có hai người
                    .FirstOrDefaultAsync();

                if (existingConversation != null)
                {
                    var conversationDtoExist = _mapper.Map<ConversationDto>(existingConversation);

                    // Tìm ID của người dùng kia (người đang được nhắn tin)
                    var otherUserId = existingConversation.Participants
                        .FirstOrDefault(id => id != currentUserId);

                    // Lấy thông tin người dùng kia
                    var otherUser = await _userService.GetUserById(otherUserId);
                    conversationDtoExist.Name = otherUser.Name; // Gán tên của người kia cho cuộc trò chuyện

                    return new ApiResult<ConversationDto>
                    {
                        Status = true,
                        Message = "A personal conversation between these two users already exists.",
                        Data = conversationDtoExist
                    };
                }
            }

            // Nếu không tồn tại cuộc trò chuyện nào thì tiến hành tạo mới
            var conversation = _mapper.Map<CreateConversationRequest, Conversation>(request);

            conversation.CreatedAt = DateTime.Now;

            if (request.AvatarUrlFile?.Length > 0)
            {
                conversation.AvatarUrl = await _fileService.UploadFileAsync(request.AvatarUrlFile, PathConstants.ConversattionAvatar);
            }

            conversation.Censor = false;

            await _context.Conversations.InsertOneAsync(conversation);

            var conversationDto = _mapper.Map<ConversationDto>(conversation);

            return new ApiResult<ConversationDto>
            {
                Status = true,
                Message = "Successful",
                Data = conversationDto
            };
        }


        [HttpPut("update-info")]
        public async Task<ApiResult<bool>> UpdateConversation([FromBody] UpdateInfoConversationRequest updateRequest)
        {
            var conversation = await _context.Conversations.Find(conversation => conversation.Id == updateRequest.Id).FirstOrDefaultAsync();
            if (conversation == null) 
                return new ApiResult<bool>
                {
                    Status=false,
                    Message="Not found",
                    Data=false
                };

            _mapper.Map(updateRequest, conversation);

            if (updateRequest.AvatarUrlFile?.Length>0)
            {
                await _fileService.DeleteFileAsync(conversation.AvatarUrl);
                await _fileService.UploadFileAsync(updateRequest.AvatarUrlFile, PathConstants.ConversattionAvatar);
            }

            var result = await _context.Conversations.ReplaceOneAsync(c => c.Id == updateRequest.Id, conversation);

            if (result.MatchedCount == 0) return new ApiResult<bool>
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

        [HttpPut("update-participants")]
        public async Task<IActionResult> UpdateParticipants([FromBody] UpdateParticipantsConversationRequest updateRequest)
        {
            // Tìm Conversation hiện tại trong cơ sở dữ liệu
            var conversation = await _context.Conversations.Find(conversation => conversation.Id == updateRequest.Id).FirstOrDefaultAsync();

            if (conversation == null)
            {
                return NotFound();
            }

            // Kiểm tra xem có các Participants đã tồn tại trong danh sách không
            var newParticipants = updateRequest.Participants.Where(p => !conversation.Participants.Contains(p)).ToList();

            // Thêm các Participants mới vào danh sách hiện tại
            if (newParticipants.Count > 0)
            {
                conversation.Participants.AddRange(newParticipants);
            }

 

            // Cập nhật trong cơ sở dữ liệu
            var result = await _context.Conversations.ReplaceOneAsync(c => c.Id == updateRequest.Id, conversation);

            if (result.MatchedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }


        [HttpDelete("delete-participants")]
        public async Task<IActionResult> DeleteParticipants([FromBody] DeleteParticipantsConversationRequest deleteRequest)
        {
            // Tìm kiếm cuộc trò chuyện trong cơ sở dữ liệu dựa trên ID
            var conversation = await _context.Conversations.Find(c => c.Id == deleteRequest.Id).FirstOrDefaultAsync();

            if (conversation == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy cuộc trò chuyện
            }

            // Loại bỏ các participants từ danh sách hiện tại nếu có
            var participantsToRemove = deleteRequest.Participants.Where(p => conversation.Participants.Contains(p)).ToList();
            if (participantsToRemove.Count > 0)
            {
                // Xóa những participants đã tồn tại trong danh sách
                conversation.Participants = conversation.Participants.Except(participantsToRemove).ToList();
            }
            else
            {
                return BadRequest("No participants to remove or participants not found in conversation."); // Không có participants để xóa
            }

            // Cập nhật cuộc trò chuyện trong cơ sở dữ liệu
            var result = await _context.Conversations.ReplaceOneAsync(c => c.Id == deleteRequest.Id, conversation);

            if (result.MatchedCount == 0)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy cuộc trò chuyện
            }

            return NoContent(); // Trả về 204 khi xóa thành công
        }




        [HttpDelete("delete")]
        public async Task<ApiResult<bool>> DeleteConversation([FromBody] DeleteRequest request)
        {
            var result = await _context.Conversations.DeleteOneAsync(conversation => conversation.Id == request.Id);
            if (result.DeletedCount == 0) return new ApiResult<bool>
            {
                Status=false,
                Message="Not Found",
                Data=false,
            };
            return new ApiResult<bool>
            {
                Status = true,
                Message="Successful",
                Data=true,
            };
        }
    }
}
