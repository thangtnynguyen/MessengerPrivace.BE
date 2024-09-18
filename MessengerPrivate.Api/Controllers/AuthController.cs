using Amazon.Runtime.Internal;
using AutoMapper;
using MessengerPrivate.Api.Constants;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.Auth;
using MessengerPrivate.Api.Models.Common;
using MessengerPrivate.Api.Models.User;
using MessengerPrivate.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MessengerPrivate.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {


        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly UserService _userService;
        private readonly IMapper _mapper;


        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config, UserService userService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ApiResult<bool>> Register([FromForm] CreateUserRequest request)
        {

            //var user = new User { UserName = email, Email = email };

            var user = _mapper.Map<CreateUserRequest, User>(request);
            user.AvatarUrl = AvatarDefault.AvatarDefaultPath;
            user.UserName = request.Email;

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return new ApiResult<bool>()
                {
                    Status = true,
                    Message = "Successful",
                    Data = true
                };

            }

            return new ApiResult<bool>()
            {
                Status = false,
                Message = "Fail!",
                Data = false
            };
        }

        [HttpPost("logout")]
        public async Task<ApiResult<bool>> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();

                var user = await _userService.GetUserInfoAsync();

                await this.RevokeRefreshToken(user.UserName);

                return new ApiResult<bool>()
                {
                    Status = true,
                    Message = "Successful",
                    Data = true
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
            catch (BadHttpRequestException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpPost("login")]
        public async Task<ApiResult<LoginResult>> Login(LoginRequest request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, true);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                var accessToken = GenerateJwtToken(request.Email, user);
                var refreshToken = GenerateRefreshToken();


                var loginResult = new LoginResult()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Expiration = DateTime.Now,
                };

                return new ApiResult<LoginResult>()
                {
                    Status = true,
                    Message = "Successful",
                    Data = loginResult
                };
            }
            return new ApiResult<LoginResult>()
            {
                Status = false,
                Message = "Fail!",
                Data = null
            };
        }

        private string GenerateJwtToken(string email, User user)
        {

            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimType.Email, email),
                    new Claim(ClaimType.Id, user.Id.ToString())

            };

            var key = _config["JwtConfig:Secret"];

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Không thể tải cấu hình Key Jwt!");
            }

            var minuteValidToken = _config["JwtConfig:TokenValidityInMinutes"];

            if (string.IsNullOrEmpty(minuteValidToken))
            {
                throw new ArgumentNullException(nameof(minuteValidToken), "Không thể tải cấu hình TokenValidityInMinutes Jwt!");
            }

            var issuer = _config["JwtConfig:Issuer"];

            if (string.IsNullOrEmpty(issuer))
            {
                throw new ArgumentNullException(nameof(key), "Không thể tải cấu hình Issuer Jwt!");
            }

            var audience = _config["JwtConfig:Audience"];

            if (string.IsNullOrEmpty(audience))
            {
                throw new ArgumentNullException(nameof(key), "Không thể tải cấu hình Audience Jwt!");
            }

            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(int.Parse(minuteValidToken));


            var token = new JwtSecurityToken(
                         issuer: issuer,
                         audience: audience,
                         expires: expires,
                         claims: claims,
                         signingCredentials: creds
                      );

            var tokenHandler = new JwtSecurityTokenHandler();

            string tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }



        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }


        private async Task<bool> RevokeRefreshToken(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                throw new BadHttpRequestException("Username does not exist in the system!");
            }

            user.RefreshToken = null;

            await _userManager.UpdateAsync(user);

            return true;
        }
    }
}
