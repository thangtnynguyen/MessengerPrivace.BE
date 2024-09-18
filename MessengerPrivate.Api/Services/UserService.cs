using AutoMapper;
using MessengerPrivate.Api.Constants;
using MessengerPrivate.Api.Data;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.User;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System.Security.Claims;

namespace MessengerPrivate.Api.Services
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MongoDbContext _dbContext;
        private readonly IMapper _mapper;



        public UserService(IHttpContextAccessor httpContextAccessor, MongoDbContext dbContext, IMapper mapper) {

            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _mapper = mapper;



        }


        public async Task<UserDto> GetUserInfo(HttpContext httpContext)
        {
            try
            {


                var id = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimType.Id);
                var email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimType.Email);


                if (id != null)
                {
                    Guid userId = Guid.Parse(id.Value.ToString());


                    var user = await _dbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync(); // Thay đổi ở đây

                    //var user = await _dbContext.Users.Find(x => x.Email == email.Value).FirstOrDefaultAsync(); // Thay đổi ở đây


                    if (user == null)
                    {
                        return null;
                    }

                    //var roles = await this.GetRoleByUserAsync(user);

                    var userDto = _mapper.Map<UserDto>(user);


                    //userDto.Roles = roles;

                    return userDto;

                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserDto> GetUserInfoAsync()
        {
            try
            {
                // Lấy ID người dùng từ claims
                var idClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimType.Id);

                Console.WriteLine("idClaim: " + idClaim?.Value); // Log giá trị của idClaim

                if (idClaim != null)
                {
                    Guid userId = Guid.Parse(idClaim.Value);

                    // Truy vấn MongoDB để tìm người dùng dựa trên ID
                    //var user = await _dbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync(); // Thay đổi ở đây

                    //var user = await _dbContext.Users.Find(x => x.Id.ToString() == userId.ToString()).FirstOrDefaultAsync();
                    var user = await _dbContext.Users.Find(x => x.Id == userId).FirstOrDefaultAsync(); // Thay đổi ở đây



                    // Chuyển đổi người dùng thành UserDto

                    var userDto = _mapper.Map<UserDto>(user);


                    //userDto.Roles = roles;

                    return userDto;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<UserDto> GetUserById(Guid userId)
        {
            var user = await _dbContext.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            var userDto = _mapper.Map<UserDto>(user);

            return userDto;
        }

    }

}
