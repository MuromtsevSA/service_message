using service_message.Models;

namespace service_message.Repository
{
    public interface IMessageRepository
    {
        Task CreateAsync(Message message);
        Task<IEnumerable<Message>> GetMessagesSinceAsync(DateTime since);
        void InitializeDatabase();
    }
}
