using service_message.Dto;
using service_message.Models;

namespace service_message.Service
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetRecentMessages();
        Task<Message> CreateMessage(MessageDto messageDto); 
    }
}
