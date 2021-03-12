using System.Linq;
using System;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entites;
using API.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using API.Interfaces;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private IMapper _mapper;
        private IUnitOfWork _unitOfWfork;
        private IHubContext<PresenceHub> _presenceHub;
        private presenceTracker _presenceTracker;

        public MessageHub(IUnitOfWork unitOfWork, IMapper mapper,
        IHubContext<PresenceHub> presenceHub, presenceTracker presenceTracker)
        {
            _mapper = mapper;
            _unitOfWfork=unitOfWork;
            _presenceHub=presenceHub;
            _presenceTracker=presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {

            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
           var groupe= await addToGroup(groupName);
           await Clients.Group(groupName).SendAsync("UpdatedGroupe", groupe);
           
            var messages = await _unitOfWfork.MessageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);
            if(_unitOfWfork.HasChanges())  await _unitOfWfork.Complete();
            
            await Clients.Caller.SendAsync("ReceivedMessagesThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group=await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdateGroupe", group);
            await base.OnDisconnectedAsync(exception);
        }


        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";

        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var senderUsername = Context.User.GetUsername();

            if (senderUsername == createMessageDto.RecipientUsername.ToLower()) throw new HubException("you can't send message to yourself");

            var sender = await _unitOfWfork.UserRepository.GetUserByUsernameAsync(senderUsername);
            var recipient = await _unitOfWfork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) throw new HubException();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = senderUsername,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            var grpName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _unitOfWfork.MessageRepository.GetMessageGroup(grpName);

            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }else{
                var connections= await _presenceTracker.GetConnectionsForUSer(recipient.UserName);
                
                if(connections!=null) {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                    new { senderUsername=sender.UserName, knownAs= sender.KnownAs});
                }
            }

            _unitOfWfork.MessageRepository.AddMessage(message);

            if (await _unitOfWfork.Complete())
            {

                await Clients.Group(grpName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }

        private async Task<Group> addToGroup(string groupName)
        {
            var grpe = await _unitOfWfork.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (grpe == null)
            {
                grpe = new Group(groupName);
                _unitOfWfork.MessageRepository.AddGroup(grpe);
            }
            grpe.Connections.Add(connection);
            if(await _unitOfWfork.Complete())  return grpe;

            throw new  HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _unitOfWfork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection= group.Connections.FirstOrDefault(c =>c.ConnectionId== Context.ConnectionId);
        
            _unitOfWfork.MessageRepository.RemoveConnection(connection);
           if(await _unitOfWfork.Complete()) return group;
           
            throw new HubException("Failed to remove group");
        }
    }
}