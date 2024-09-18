using AutoMapper;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.CallSession;
using MessengerPrivate.Api.Models.Common;
using MessengerPrivate.Api.Models.Conversation;
using MessengerPrivate.Api.Services;
using MessengerPrivate.Api.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MessengerPrivate.Api.Controllers
{
    [Route("api/call-session")]
    [ApiController]
    public class CallSessionController : ControllerBase
    {
        private readonly IHubContext<CallVideoHub> _callVideoHub;
        private readonly IHubContext<CallSessionHub> _callSesionHub;
        private readonly IMapper _mapper;
        private readonly FileService _fileService;
        private readonly CallSessionService _callSessionService;
        private readonly UserService _userService;

        public CallSessionController(CallSessionService callSessionService, IHubContext<CallVideoHub> hubContext, IMapper mapper, FileService fileService, UserService userService, IHubContext<CallSessionHub> callSesionHub)
        {
            _callSessionService = callSessionService;
            _callVideoHub = hubContext;
            _mapper = mapper;
            _fileService = fileService;
            _userService = userService;
            _callSesionHub = callSesionHub;
        }

        [HttpPost("create")]
        public async Task<ApiResult<CallSessionDto>> CreateCallSessionAsync([FromBody] CreateCallSessionRequest request)
        {
           var callSessionDto= await _callSessionService.CreateCallSessionAsync(request);

            if (callSessionDto == null)
            {
                return new ApiResult<CallSessionDto>
                {
                    Status=false,
                    Message="Fail",
                    Data=null
                };
            }
            await _callVideoHub.Clients.Group(request.ConversationId).SendAsync("CallVideoRequest", callSessionDto);
            //await _callVideoHub.Clients.GroupExcept(request.ConversationId, new[] { Context.ConnectionId }).SendAsync("CallVideoRequest", callSessionDto);

            return new ApiResult<CallSessionDto>
            {
                Status = true,
                Message = "Succesful",
                Data = callSessionDto
            };

        }

        [HttpPut("add-user")]
        public async Task<ApiResult<CallSessionDto>> AddUserToCallSessionAsync([FromBody] AddUserCallVideoRequest request)
        {
            var callSessionDto = await _callSessionService.AddUserToCallSessionAsync(request);

            if (callSessionDto == null)
            {
                return new ApiResult<CallSessionDto>
                {
                    Status = false,
                    Message = "Fail",
                    Data = null
                };
            }
            await _callVideoHub.Clients.Group(request.ConversationId).SendAsync("AcceptCallVideoRequest", callSessionDto);
            await _callSesionHub.Clients.Group(request.CallSessionId).SendAsync("JoinCallVideo", callSessionDto);

            return new ApiResult<CallSessionDto>
            {
                Status = true,
                Message = "Succesful",
                Data = callSessionDto
            };
        }

        [HttpPut("update-user")]
        public async Task<ApiResult<CallSessionDto>> UpdateUserCallVideoStatusAsync([FromBody]  UpdateUserCallVideoRequest request)
        {
            var user = await _userService.GetUserInfoAsync();
            var callSessionDto = await _callSessionService.UpdateUserCallVideoStatusAsync(request);

            if (callSessionDto == null)
            {
                return new ApiResult<CallSessionDto>
                {
                    Status = false,
                    Message = "Fail",
                    Data = null
                };
            }
            if (request.Type == "mutecamera")
            {
                await _callVideoHub.Clients.Group(request.ConversationId).SendAsync("OnMuteCamera", user);

            }
            await _callVideoHub.Clients.Group(request.ConversationId).SendAsync("OnMuteMicro", user);


            return new ApiResult<CallSessionDto>
            {
                Status = true,
                Message = "Succesful",
                Data = callSessionDto
            };
        }

        [HttpPut("update-end-time")]
        public async Task<ApiResult<CallSessionDto>> EndCallSessionAsync([FromBody] UpdateEndTimeCallVideoRequest request)
        {
            var callSessionDto = await _callSessionService.EndCallSessionAsync(request);

            if (callSessionDto == null)
            {
                return new ApiResult<CallSessionDto>
                {
                    Status = false,
                    Message = "Fail",
                    Data = null
                };
            }
            await _callVideoHub.Clients.Group(request.ConversationId).SendAsync("OnEndCallVideo", callSessionDto);

            return new ApiResult<CallSessionDto>
            {
                Status = true,
                Message = "Succesful",
                Data = callSessionDto
            };
        }


    }
}
