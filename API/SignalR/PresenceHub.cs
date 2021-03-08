using System;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub: Hub
    {
        private presenceTracker _tracker;

        public PresenceHub(presenceTracker tracker){
            _tracker= tracker;
        }
        public override async Task OnConnectedAsync()
        {
            var isOnline=await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            if(isOnline)
                await Clients.Others.SendAsync("UserIsOnLine", Context.User.GetUsername());

            var currentUsers= await _tracker.GetOnelineUSers();
            await Clients.Caller.SendAsync("OnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline=await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
            if(isOffline)
                await Clients.Others.SendAsync("UserIsOffLine", Context.User.GetUsername());

            await base.OnDisconnectedAsync(exception);
        }
    }
}