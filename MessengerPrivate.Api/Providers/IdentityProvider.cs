using MessengerPrivate.Api.Data.Entities;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using MongoDbGenericRepository;

namespace MessengerPrivate.Api.Providers
{
    public static class IdentityProvider
    {
        public static IServiceCollection AddIdentityProvider(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.Configure<MessengerPrivate.Api.Data.MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
            services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<MessengerPrivate.Api.Data.MongoDbSettings>>().Value);


            var mongoDbSettings = new MessengerPrivate.Api.Data.MongoDbSettings
            {
                ConnectionString = builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Value,
                DatabaseName = builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value
            };


            // Add MongoDB context
            services.AddSingleton<MessengerPrivate.Api.Data.MongoDbContext>(provider =>
                   new MessengerPrivate.Api.Data.MongoDbContext(mongoDbSettings.ConnectionString, mongoDbSettings.DatabaseName));     
            
            //services.AddSingleton<IMongoDbContext>(provider =>
            //     new MessengerPrivate.Api.Data.MongoDbContext(mongoDbSettings.ConnectionString, mongoDbSettings.DatabaseName));

            //var mongoDbSettings = builder.Configuration.GetSection(nameof(MessengerPrivate.Api.Data.MongoDbSettings)).Get<MessengerPrivate.Api.Data.MongoDbSettings>();
            services.AddIdentity<User, Data.Entities.Role>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1; // Đặt độ dài tối thiểu
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.AllowedUserNameCharacters = null; // Cho phép tất cả ký tự trong tên người dùng
            })
            .AddMongoDbStores<User, Data.Entities.Role, Guid>(
                mongoDbSettings.ConnectionString,
                mongoDbSettings.DatabaseName)
            .AddDefaultTokenProviders();


            // Configure JWT Authentication
            var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);

            var jwtKey = builder.Configuration.GetValue<string>("JwtConfig:Key");
            var audience = builder.Configuration.GetValue<string>("JwtConfig:Audience");
            var issuer = builder.Configuration.GetValue<string>("JwtConfig:Issuer");

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    ValidAudience = audience,
                    ValidIssuer = issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                };


                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                                   (path.StartsWithSegments("/hubs/messenger") ||
                                    path.StartsWithSegments("/hubs/call-video") || path.StartsWithSegments("/hubs/call-session")))
                        {
                            
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };

               


            });


            return services;



        }
    }

}
