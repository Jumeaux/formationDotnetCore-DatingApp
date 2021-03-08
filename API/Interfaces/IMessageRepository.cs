using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entites;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        
        void AddGroup(Group group);
        Task<Group> GetMessageGroup(string groupName);

        void RemoveConnection(Connection Connection);
        Task<Connection> GetConnection(string ConnectionId);
        void AddMessage(Message message);
        void DeleteMessage(Message message);

        Task<Message> GetMessage(int id);

        Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams);

        Task<IEnumerable<MessageDto>>  GetMessageThread(string currentUsername, string recipeientUsername);

        Task<Group> GetGroupForConnection(string connectionId);
        Task<bool> SaveAllAsync();
    }
}