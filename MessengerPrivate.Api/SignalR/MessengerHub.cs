using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;

namespace MessengerPrivate.Api.SignalR
{
    [Authorize]
    public class MessengerHub:Hub
    {

        //private readonly IHubContext<PresenceHub> _presenceHub;
        //private readonly PresenceTracker _presenceTracker;
        //private readonly UserShareScreenTracker _shareScreenTracker;
        //private readonly UserService _userService;
        //private readonly GroupService _groupService;

        //public MessengerHub(UserShareScreenTracker shareScreenTracker, PresenceTracker presenceTracker, IHubContext<PresenceHub> presenceHub, UserService userService, GroupService groupService)
        //{
        //    _presenceTracker = presenceTracker;
        //    _presenceHub = presenceHub;
        //    _shareScreenTracker = shareScreenTracker;
        //    _userService = userService;
        //    _groupService = groupService;
        //}


        public MessengerHub()
        {
          
        }

        public async Task JoinConversationGroup(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            await Clients.Caller.SendAsync("JoinConversationGroup", conversationId);
        }

        public async Task LeaveConversationGroup(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
            await Clients.Caller.SendAsync("LeaveConversationGroup", conversationId);
        }


    }
}
