using MessengerPrivate.Api.Extensions;
using MessengerPrivate.Api.Models.CallSession;
using MessengerPrivate.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MessengerPrivate.Api.SignalR
{
    [Authorize]
    public class CallSessionHub:Hub
    {
        IHubContext<PresenceHub> _presenceHub;
        PresenceTracker _presenceTracker;
        UserShareScreenTracker _shareScreenTracker;
        UserService _userService;
        public CallSessionHub(UserShareScreenTracker shareScreenTracker, PresenceTracker presenceTracker, IHubContext<PresenceHub> presenceHub, UserService userService)
        {

            _presenceTracker = presenceTracker;
            _presenceHub = presenceHub;
            _shareScreenTracker = shareScreenTracker;
            _userService = userService;
        }

        public async Task JoinCallSessionGroup(string callSessionId)
        {
            var user = await _userService.GetUserInfoAsync();
            await Groups.AddToGroupAsync(Context.ConnectionId, callSessionId);
            await Clients.Group(callSessionId).SendAsync("UserOnlineInGroup", user);
            await Clients.Caller.SendAsync("JoinCallSessionGroup", callSessionId);

        }

        public async Task LeaveCallSessionGroup(string callSessionId, Exception exception)
        {
            var user = await _userService.GetUserInfoAsync();

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, callSessionId.ToString());
            await Clients.Group(callSessionId).SendAsync("UserOfflineOutGroup", user);
            await Clients.Caller.SendAsync("LeaveCallSessionGroup", callSessionId);

            await base.OnDisconnectedAsync(exception);
        }


        public async Task MuteMicro(bool muteMicro, string conversationId)
        {

            await Clients.Group(conversationId.ToString()).SendAsync("OnMuteMicro", new { username = Context.User.GetUsername(), mute = muteMicro });

        }

        public async Task MuteCamera(bool muteCamera, string conversationId)
        {

            await Clients.Group(conversationId.ToString()).SendAsync("OnMuteCamera", new { username = Context.User.GetUsername(), mute = muteCamera });

        }
    

    }
}
