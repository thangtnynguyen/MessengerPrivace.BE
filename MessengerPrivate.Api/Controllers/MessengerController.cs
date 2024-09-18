using Microsoft.AspNetCore.Mvc;
using MessengerPrivate.Api.Data;
using MessengerPrivate.Api.Data.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using AutoMapper;
using MessengerPrivate.Api.Models.Common;
using MessengerPrivate.Api.Models.Messenger;
using Microsoft.IdentityModel.Tokens;
using MessengerPrivate.Api.Services;
using MessengerPrivate.Api.Constants;
using MessengerPrivate.Api.SignalR;
using MessengerPrivate.Api.Models.Icon;

namespace MessengerPrivate.Api.Controllers
{
    [Route("api/messenger")]
    [ApiController]
    public class MessengerController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly IHubContext<MessengerHub> _messengerHub;
        private readonly IMapper _mapper;
        private readonly FileService _fileService;

        public MessengerController(MongoDbContext context, IHubContext<MessengerHub> hubContext, IMapper mapper, FileService fileService)
        {
            _context = context;
            _messengerHub = hubContext;
            _mapper = mapper;
            _fileService = fileService;
        }

        [HttpPost("send")]
        public async Task<ApiResult<MessengerDto>> SendMessage([FromForm] SendMessengerRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ConversationId))
            {
                return new ApiResult<MessengerDto>
                {
                    Status = false,
                    Message = "Message or ConversationId cannot be null",
                    Data = null
                };
            }

            var messenger = _mapper.Map<SendMessengerRequest, Messenger>(request);
            messenger.SentTime = DateTime.Now;
            messenger.CreatedAt = DateTime.Now;
            messenger.CreatedBy = request.SenderId;

            var iconDto= _mapper.Map<IconRequest,IconDto>(request.Icon);
            messenger.Icon=iconDto;

            if (request.MediaFiles != null && request.MediaFiles.Count > 0)
            {
                messenger.Medias = new List<Media>();

                foreach (var mediaFileRequest in request.MediaFiles)
                {
                    var fileName = "";

                    // Dựa vào thuộc tính Type để xác định đường dẫn lưu trữ file
                    switch (mediaFileRequest.Type)
                    {
                        case "file":
                            fileName = await _fileService.UploadFileAsync(mediaFileRequest.File, PathConstants.MediaFile);
                            break;
                        case "image":
                            fileName = await _fileService.UploadFileAsync(mediaFileRequest.File, PathConstants.MediaImage);
                            break;
                        case "video":
                            fileName = await _fileService.UploadFileAsync(mediaFileRequest.File, PathConstants.MediaVideo);
                            break;
                        case "audio":
                            fileName = await _fileService.UploadFileAsync(mediaFileRequest.File, PathConstants.MediaAudio);
                            break;
                        case "pdf":
                            fileName = await _fileService.UploadFileAsync(mediaFileRequest.File, PathConstants.MediaPdf);
                            break;
                        case "word":
                            fileName = await _fileService.UploadFileAsync(mediaFileRequest.File, PathConstants.MediaWord);
                            break;
                        case "excel":
                            fileName = await _fileService.UploadFileAsync(mediaFileRequest.File, PathConstants.MediaExcel);
                            break;
                        case "powerpoint":
                            fileName = await _fileService.UploadFileAsync(mediaFileRequest.File, PathConstants.MediaPowerpoint);
                            break;
                        default:
                            fileName = await _fileService.UploadFileAsync(mediaFileRequest.File, PathConstants.MediaOther);
                            break;
                    }

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        messenger.Medias.Add(new Media
                        {
                            Type = mediaFileRequest.Type,
                            Url = fileName,
                            Name=mediaFileRequest.Name,
                        });
                    }
                }
            }

            await _context.Messengers.InsertOneAsync(messenger);
            await _messengerHub.Clients.Group(messenger.ConversationId).SendAsync("ReceiveMessenger", messenger);

            var messengerDto = _mapper.Map<MessengerDto>(messenger);

            return new ApiResult<MessengerDto>
            {
                Status = true,
                Message = "Successfully",
                Data = messengerDto
            };


        }


        [HttpGet("get-by-conversation")]
        public async Task<ApiResult<PagingResult<MessengerDto>>> GetMessagesByConversationId([FromQuery] GetMessengerByConversationIdRequest request)
        {
            if (string.IsNullOrEmpty(request.ConversationId))
            {
                return new ApiResult<PagingResult<MessengerDto>>
                {
                    Status = false,
                    Message = "ConversationId cannot be null",
                    Data = null
                };
            }

            var filter = Builders<Messenger>.Filter.Eq(m => m.ConversationId, request.ConversationId);

            var totalRecords = await _context.Messengers.CountDocumentsAsync(filter);

            // Mặc định sắp xếp theo SentTime nếu không có orderBy
            var sortField = !string.IsNullOrEmpty(request.OrderBy) ? request.OrderBy : nameof(Messenger.SentTime);

            // Kiểm tra giá trị của SortBy (asc hoặc desc)
            var sortDefinition = Builders<Messenger>.Sort.Descending(sortField); // Mặc định là giảm dần
            if (!string.IsNullOrEmpty(request.SortBy) && request.SortBy.ToLower() == "asc")
            {
                sortDefinition = Builders<Messenger>.Sort.Ascending(sortField);
            }

            var skip = (request.PageIndex - 1) * request.PageSize;

            var messages = await _context.Messengers
                .Find(filter)
                .Sort(sortDefinition)
                .Skip(skip)
                .Limit(request.PageSize)
                .ToListAsync();

            if (messages == null || messages.Count == 0)
            {
                return new ApiResult<PagingResult<MessengerDto>>
                {
                    Status = false,
                    Message = "No messages found for this conversation",
                    Data = null
                };
            }

            var messengerDtos = _mapper.Map<List<MessengerDto>>(messages);

            var pagingResult = new PagingResult<MessengerDto>(messengerDtos, request.PageIndex ?? 1, request.PageSize ?? 10, (int)totalRecords);

            return new ApiResult<PagingResult<MessengerDto>>
            {
                Status = true,
                Message = "Successfully",
                Data = pagingResult
            };
        }




        [HttpPut("update-messenger")]
        public async Task<ApiResult<MessengerDto>> UpdateMessage([FromBody] UpdateMessengerRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Id))
                return new ApiResult<MessengerDto>
                {
                    Status = false,
                    Message = "Message or ID cannot be null",
                    Data = null
                };

            var filter = Builders<Messenger>.Filter.Eq(m => m.Id, request.Id);
            var update = Builders<Messenger>.Update
                .Set(m => m.Content, request.Content)
                .Set(m => m.MessengerType, request.MessengerType)
                .Set(m => m.SentTime, DateTime.Now);

            var result = await _context.Messengers.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                return new ApiResult<MessengerDto>
                {
                    Status = false,
                    Message = "Message not found",
                    Data = null
                };

            await _messengerHub.Clients.Group(request.ConversationId).SendAsync("MessageUpdated",true);


            var messengerDto = _mapper.Map<MessengerDto>(request);
            return new ApiResult<MessengerDto>
            {
                Status=true,
                Message= "Successfully",
                Data = messengerDto
            };
        }

        [HttpPut("update-status")]
        public async Task<ApiResult<bool>> UpdateStatus( [FromBody] UpdateMessengerStatusRequest request)
        {
            if (string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.Status))

                return new ApiResult<bool>
                {
                    Status=false,
                    Message= "ID and Status cannot be null or empty",
                    Data=false
                };

            var filter = Builders<Messenger>.Filter.Eq(m => m.Id, request.Id);

            var update = Builders<Messenger>.Update.Set(m => m.Status, request.Status).Set(m => m.UpdatedAt, DateTime.Now); ;

            var result = await _context.Messengers.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)

                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Messenger not found",
                    Data = false
                };

            await _messengerHub.Clients.Group(request.ConversationId).SendAsync("MessengerUpdated",true);

            return new ApiResult<bool>
            {
                Status = true,
                Message = "Successfully",
                Data = true
            };
        }


        [HttpDelete("delete")]
        public async Task<ApiResult<bool>> DeleteMessage([FromBody] DeleteRequest request)
        {
            if (string.IsNullOrEmpty(request.Id))
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Message not found",
                    Data = false
                };

            var filter = Builders<Messenger>.Filter.Eq(m => m.Id, request.Id);
            var message = await _context.Messengers.Find(filter).FirstOrDefaultAsync();

            if (message == null)
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Message not found",
                    Data = false
                };

            await _context.Messengers.DeleteOneAsync(filter);

            await _messengerHub.Clients.Group(message.ConversationId).SendAsync("MessengerDeleted", request.Id);

            return new ApiResult<bool>
            {
                Status = true,
                Message = "Successfully",
                Data = true
            };

        }

    }
}
