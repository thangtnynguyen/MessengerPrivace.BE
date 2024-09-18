using MessengerPrivate.Api.Providers;
using MessengerPrivate.Api.SignalR;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddIdentityProvider(builder);
builder.Services.AddDependencyInjectionProvider();
builder.Services.AddSwaggerProvider();
builder.Services.AddAutoMapperProvider();
builder.Services.AddEndpointsApiExplorer();



//add signltion
builder.Services.AddSingleton<PresenceTracker>();
builder.Services.AddSingleton<UserShareScreenTracker>();

// Add SignalR
builder.Services.AddSignalR();

// Configure JSON options to handle cycles and indentation
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

var app = builder.Build();

// Configure CORS before Authentication and Authorization
app.UseCors(builder => builder
    .WithOrigins("http://localhost:4200", "https://tnymessenger.click", "https://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

// Routing middleware
app.UseRouting();

// Authentication middleware
app.UseAuthentication();

// Authorization middleware should be after UseRouting and before UseEndpoints
app.UseAuthorization();

// Map endpoints after routing is configured
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<SignalingHub>("/hubs/signaling");
    endpoints.MapHub<PresenceHub>("/hubs/presence");
    endpoints.MapHub<CallVideoHub>("/hubs/call-video");
    endpoints.MapHub<MessengerHub>("/hubs/messenger");
    endpoints.MapHub<CallSessionHub>("/hubs/call-session");

});

app.Run();









////using MeetingAppCore.SignalR;
////using MessengerPrivate.Api.Providers;
////using MessengerPrivate.Api.SignalR;
////using System.Text.Json.Serialization;

////var builder = WebApplication.CreateBuilder(args);

////// Add services to the container.



//////builder.Services.AddControllers().AddJsonOptions(options =>
//////{
//////    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//////    options.JsonSerializerOptions.WriteIndented = true;
//////});


//////builder.Services.AddEndpointsApiExplorer();
////builder.Services.AddHttpClient();

////builder.Services.AddIdentityProvider(builder);
////builder.Services.AddDependencyInjectionProvider();
////builder.Services.AddSwaggerProvider();
////builder.Services.AddAutoMapperProvider();


////builder.Services.AddEndpointsApiExplorer();



//////builder.Services.AddControllers();
////// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

////builder.Services.AddSignalR();

////builder.Services.AddControllers().AddJsonOptions(options =>
////{
////    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
////    options.JsonSerializerOptions.WriteIndented = true;
////});


//////builder.Services.AddSwaggerGen();

////var app = builder.Build();

////app.UseCors(builder => builder
////    //.AllowAnyOrigin()
////    .WithOrigins("http://localhost:4200", "https://tnymessenger.click", "https://localhost:4200")
////    .AllowAnyMethod()
////    .AllowAnyHeader()
////    .AllowCredentials()

////    );

////// Configure the HTTP request pipeline.
////if (app.Environment.IsDevelopment())
////{
////    app.UseSwagger();
////    app.UseSwaggerUI();
////}

////app.UseHttpsRedirection();

////app.UseAuthentication();

////app.UseAuthorization();

////app.UseRouting(); 


////app.UseEndpoints(endpoints =>
////{
////    endpoints.MapControllers();
////    endpoints.MapHub<SignalingHub>("hubs/signaling");
////    endpoints.MapHub<PresenceHub>("hubs/presence");
////    endpoints.MapHub<ChatHub>("hubs/chathub");
////    endpoints.MapHub<MessengerHub>("/hubs/messenger");

////    //endpoints.MapFallbackToController("Index", "Fallback");//publish

////});

////app.MapControllers();

////app.Run();
