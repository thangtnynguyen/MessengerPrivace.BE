using AutoMapper;
using MessengerPrivate.Api.Constants;
using MessengerPrivate.Api.Data;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.Common;
using MessengerPrivate.Api.Models.Icon;
using MessengerPrivate.Api.Models.Messenger;
using MessengerPrivate.Api.Services;
using MessengerPrivate.Api.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace MessengerPrivate.Api.Controllers
{
    [Route("api/icon")]
    [ApiController]
    public class IconController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly IHubContext<MessengerHub> _messengerHub;
        private readonly IMapper _mapper;
        private readonly FileService _fileService;

        public IconController(MongoDbContext context, IHubContext<MessengerHub> hubContext, IMapper mapper, FileService fileService)
        {
            _context = context;
            _messengerHub = hubContext;
            _mapper = mapper;
            _fileService = fileService;
        }

        [HttpGet("get-paging")]
        public async Task<ApiResult<PagingResult<IconDto>>> GetMessagesByConversationId([FromQuery] GetIconRequest request)
        {
            var builder = Builders<Icon>.Filter;

            FilterDefinition<Icon> filter;
            if (!string.IsNullOrEmpty(request.Name))
            {
                filter = builder.Eq(m => m.Name, request.Name);
            }
            else
            {
                filter = builder.Empty;
            }

            var totalRecords = await _context.Icons.CountDocumentsAsync(filter);

            var sortDefinition = Builders<Icon>.Sort.Descending(m => m.CreatedAt);

            if (!string.IsNullOrEmpty(request.SortBy) && !string.IsNullOrEmpty(request.OrderBy))
            {
                var sortField = request.SortBy.ToLower();
                var sortOrder = request.OrderBy.ToLower() == "asc"
                    ? Builders<Icon>.Sort.Ascending(sortField)
                    : Builders<Icon>.Sort.Descending(sortField);

                sortDefinition = sortOrder;
            }

            var skip = (request.PageIndex - 1) * request.PageSize;

            var icons = await _context.Icons
                .Find(filter)
                .Sort(sortDefinition)
                .Skip(skip)
                .Limit(request.PageSize)
                .ToListAsync();

            if (icons == null || icons.Count == 0)
            {
                return new ApiResult<PagingResult<IconDto>>
                {
                    Status = false,
                    Message = "No icons found",
                    Data = null
                };
            }

            var iconDtos = _mapper.Map<List<IconDto>>(icons);

            var pagingResult = new PagingResult<IconDto>(iconDtos, request.PageIndex ?? 1, request.PageSize ?? 10, (int)totalRecords);

            return new ApiResult<PagingResult<IconDto>>
            {
                Status = true,
                Message = "Successfully",
                Data = pagingResult
            };
        }



        [HttpPost]
        [Route("create")]
        public async Task<ApiResult<bool>> Create([FromForm] CreateIconRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Name))
            {
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Name cannot be null",
                    Data = false
                };
            }

            var icon = _mapper.Map<CreateIconRequest, Icon>(request);
            icon.CreatedAt = DateTime.Now;

            if (request.UrlFile.Length > 0)
            {
                icon.Url = await _fileService.UploadFileAsync(request.UrlFile,PathConstants.Icon);
            }

            await _context.Icons.InsertOneAsync(icon);

            return new ApiResult<bool>
            {
                Status = true,
                Message = "Successfully",
                Data = true,
            };

        }


    }
}
