using Amazon.Runtime.Internal;
using AutoMapper;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Extensions;
using MessengerPrivate.Api.Models.CallSession;
using MessengerPrivate.Api.Services;
using MessengerPrivate.Api.SignalR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerPrivate.Api.SignalR
{
    [Authorize]
    public class CallVideoHub : Hub
    {
        IHubContext<PresenceHub> _presenceHub;
        PresenceTracker _presenceTracker;
        UserShareScreenTracker _shareScreenTracker;
        UserService _userService;
        public CallVideoHub( UserShareScreenTracker shareScreenTracker, PresenceTracker presenceTracker, IHubContext<PresenceHub> presenceHub, UserService userService)
        {

            _presenceTracker = presenceTracker;
            _presenceHub = presenceHub;
            _shareScreenTracker = shareScreenTracker;
            _userService = userService;
        }

        public  async Task JoinCallVideoGroup(string conversationId)
        {
            var user = await _userService.GetUserInfoAsync();
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            await Clients.Group(conversationId).SendAsync("UserOnlineInGroup", user);
            await Clients.Caller.SendAsync("JoinCallVideoGroup", conversationId);

        }

        public  async Task LeaveCallVideoGroup(string conversationId,Exception exception)
        {
            var user = await _userService.GetUserInfoAsync();

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
            await Clients.Group(conversationId).SendAsync("UserOfflineOutGroup", user);
            await Clients.Caller.SendAsync("LeaveCallVideoGroup", conversationId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task RejectCallVideoRequest(string conversationId)
        {
            var user = await _userService.GetUserInfoAsync();
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            await Clients.Group(conversationId).SendAsync("RejectCallVideoRequest", user);

        }

        public async Task MuteMicro(bool muteMicro, string conversationId)
        {
           
            await Clients.Group(conversationId.ToString()).SendAsync("OnMuteMicro", new { username = Context.User.GetUsername(), mute = muteMicro });

        }

        public async Task MuteCamera(bool muteCamera,string conversationId)
        {
           
            await Clients.Group(conversationId.ToString()).SendAsync("OnMuteCamera", new { username = Context.User.GetUsername(), mute = muteCamera });

        }
        public async Task SendRequestCallVideo( string conversationId , CallSessionDto callSessionDto)
        {
            await Clients.GroupExcept(conversationId, new[] { Context.ConnectionId }).SendAsync("CallVideoRequest", callSessionDto);
        }



    }
}
