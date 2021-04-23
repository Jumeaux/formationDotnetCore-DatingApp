using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entites;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, AutoMapper.IMapper mapper){
            _context=context;
            _mapper = mapper;
        }

        public IMapper Mapper { get; }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
           _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string ConnectionId)
        {
            return await _context.Connections.FindAsync(ConnectionId);
        }

        public Task<Group> GetGroupForConnection(string connectionId)
        {
           return _context.Groups.Include(c =>c.Connections)
            .Where(c =>c.Connections.Any(x =>x.ConnectionId==connectionId))
            .FirstOrDefaultAsync();
        }

        public  async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(u => u.Sender)
                .Include(u => u.Recipient)
                .SingleOrDefaultAsync(x =>x.Id==id);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var query= _context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox"=> query.Where(u => u.RecipientUsername== messageParams.Username && u.RecipientDelete==false),
                "Outbox"=> query.Where(u =>u.SenderUsername == messageParams.Username && u.SenderDelete==false),
                _=> query.Where(u =>u.RecipientUsername == messageParams.Username && u.DateRead== null && u.RecipientDelete==false)
            };


            return await PagedList<MessageDto>.CreateAsync(query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider),messageParams.PageNumber, messageParams.PageSize);
        }

        public  async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups.Include(c =>c.Connections)
                    .FirstOrDefaultAsync(x =>x.Name == groupName);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserUsername, string recipientUsername)
        {
            var messages= await _context.Messages
                    
                    .Where(m => m.Sender.UserName== currentUserUsername  && m.Recipient.UserName == recipientUsername && m.SenderDelete==false
                        || m.Sender.UserName== recipientUsername && m.Recipient.UserName== currentUserUsername && m.RecipientDelete==false
                  
                    ).OrderBy(m => m.MessageSent)
                    .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
                    var unreadMessages = messages.Where(m => m.DateRead==null && m.RecipientUsername == currentUserUsername);

                    if (unreadMessages.Any())
                    {
                        foreach (var message in messages)
                        {
                            message.DateRead=DateTime.UtcNow;
                        }
                        
                    }

                    return messages;
        }

        public void RemoveConnection(Connection Connection)
        {
            _context.Connections.Remove(Connection);
        }

      
    }
}