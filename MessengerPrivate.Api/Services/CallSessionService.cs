using AutoMapper;
using MessengerPrivate.Api.Data;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.CallSession;
using MongoDB.Driver;

namespace MessengerPrivate.Api.Services
{
    public class CallSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MongoDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserService _userService;



        public CallSessionService(IHttpContextAccessor httpContextAccessor, MongoDbContext dbContext, IMapper mapper, UserService userService)
        {

            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<CallSessionDto> CreateCallSessionAsync(CreateCallSessionRequest request)
        {
            var user = await _userService.GetUserInfoAsync();
            var callSession = _mapper.Map<CallSession>(request);

            callSession.UserCallVideos = new List<UserCallVideo>
            {
                new UserCallVideo
                {
                    UserId = user.Id, 
                    UserName = user.Name, 
                    UserAvatarUrl = user.AvatarUrl,  
                    MuteCamera = false, 
                    MuteMicro = false,  
                    Status = "online"   
                }
            };

            callSession.StartTime = DateTime.Now;
            callSession.CreatedAt = DateTime.Now;
            callSession.CallerId = user.Id;

            callSession.Status = "ongoing"; 

            await _dbContext.CallSessions.InsertOneAsync(callSession);

            var callSesstionDto = _mapper.Map<CallSessionDto>(callSession);   
            return callSesstionDto;
        }

        public async Task<CallSessionDto> AddUserToCallSessionAsync(AddUserCallVideoRequest request)
        {
            var callSession = await _dbContext.CallSessions.Find(x => x.Id == request.CallSessionId).FirstOrDefaultAsync();

            if (callSession == null)
            {
                return null; 
            }

            var user = await _userService.GetUserInfoAsync();

            var existingUser = callSession.UserCallVideos
                                          .FirstOrDefault(ucv => ucv.UserId == user.Id);

            if (existingUser != null)
            {
                return null;
            }

            var newUser = new UserCallVideo
            {
                UserId = user.Id,
                UserName = user.Name,
                UserAvatarUrl = user.AvatarUrl,
                MuteCamera = request.MuteCamera,
                MuteMicro = request.MuteMicro,
                Status = request.Status
            };

            callSession.UserCallVideos.Add(newUser);

            var updateResult = await _dbContext.CallSessions.ReplaceOneAsync(x => x.Id == request.CallSessionId, callSession);

            var callSesstionDto = _mapper.Map<CallSessionDto>(callSession);
            return callSesstionDto;
        }


        public async Task<CallSessionDto> UpdateUserCallVideoStatusAsync(UpdateUserCallVideoRequest request)
        {
            var callSession = await _dbContext.CallSessions.Find(x => x.Id == request.CallSessionId).FirstOrDefaultAsync();

            if (callSession == null)
            {
                return null; 
            }

            var userCallVideo = callSession.UserCallVideos.FirstOrDefault(ucv => ucv.UserId == request.UserId);

            if (userCallVideo == null)
            {
                return null; 
            }

            if (request.MuteCamera.HasValue)
            {
                userCallVideo.MuteCamera = request.MuteCamera.Value;
            }

            if (request.MuteMicro.HasValue)
            {
                userCallVideo.MuteMicro = request.MuteMicro.Value;
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                userCallVideo.Status = request.Status;
            }

            var updateResult = await _dbContext.CallSessions.ReplaceOneAsync(x => x.Id == request.CallSessionId, callSession);

            var callSesstionDto = _mapper.Map<CallSessionDto>(callSession);
            return callSesstionDto;
        }

        public async Task<CallSessionDto> EndCallSessionAsync(UpdateEndTimeCallVideoRequest request)
        {
            var callSession = await _dbContext.CallSessions.Find(x => x.Id == request.CallSessionId).FirstOrDefaultAsync();

            if (callSession == null)
            {
                return null; 
            }

            callSession.EndTime = DateTime.Now;
            callSession.Status = "offline";

            var updateResult = await _dbContext.CallSessions.ReplaceOneAsync(x => x.Id == request.CallSessionId, callSession);

            var callSesstionDto = _mapper.Map<CallSessionDto>(callSession);
            return callSesstionDto;
        }





    }
}
