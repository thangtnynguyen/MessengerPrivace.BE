using AutoMapper;
using MessengerPrivate.Api.Data;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.Common;
using MessengerPrivate.Api.Models.User;
using MessengerPrivate.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace MessengerPrivate.Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserService _userService;
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;


        public UserController( UserService userService, MongoDbContext context, IMapper mapper) { 
            _userService = userService;
            _context = context;
            _mapper = mapper;
        }


        [HttpGet("search")]
        public async Task<ApiResult<List<UserDto>>> SearchUsers([FromQuery] GetUserRequest request)
        {
            if (string.IsNullOrEmpty(request.KeyWord))
                return new ApiResult<List<UserDto>>
                {
                    Status=false,
                    Message= "Keyword cannot be empty",
                    Data=null
                };

            var filter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Regex(u => u.Name, new MongoDB.Bson.BsonRegularExpression(request.KeyWord, "i")),
                Builders<User>.Filter.Regex(u => u.PhoneNumber, new MongoDB.Bson.BsonRegularExpression(request.KeyWord, "i")),
                Builders<User>.Filter.Regex(u => u.Email, new MongoDB.Bson.BsonRegularExpression(request.KeyWord, "i"))
            );

            var users = await _context.Users.Find(filter).ToListAsync();

            if (users.Count == 0)
                return new ApiResult<List<UserDto>>
                {
                    Status = false,
                    Message = "No users found matching the given keyword",
                    Data = null
                };

            var userDtos = _mapper.Map<List<UserDto>>(users);

            return new ApiResult<List<UserDto>>
            {
                Status = true,
                Message = "Successful",
                Data = userDtos
            };
        }


        [HttpGet]
        [Route("user-info")]
        public async Task<ApiResult<UserDto>> GetUserInfo()
        {
            var user = await _userService.GetUserInfo(HttpContext);

            if (user == null)
            {
                return new ApiResult<UserDto>()
                {
                    Status = false,
                    Message = "Use not found",
                    Data = null
                };
            }

            return new ApiResult<UserDto>()
            {
                Status = true,
                Message = "Successful",
                Data = user
            };
        }

        [HttpGet]
        [Route("user-info-async")]
        public async Task<ApiResult<UserDto>> GetUserInfoAsync()
        {
            var user = await _userService.GetUserInfoAsync();

            if (user == null)
            {
                return new ApiResult<UserDto>()
                {
                    Status = false,
                    Message = "User not found",
                    Data = null
                };
            }

            return new ApiResult<UserDto>()
            {
                Status = true,
                Message = "Successful",
                Data = user
            };
        }


    }
}
