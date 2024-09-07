using service_message.Dto;
using service_message.Models;
using service_message.Repository;

namespace service_message.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;

        public MessageService(IMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Message>> GetRecentMessages()
        {
            var tenMinutesAgo = DateTime.UtcNow.AddMinutes(-10);
            return await _repository.GetMessagesSinceAsync(tenMinutesAgo);
        }


        public async Task<Message> CreateMessage(MessageDto messageDto)
        {
            var message = new Message
            {
                Text = messageDto.Text,
                Order = messageDto.Order,
                Time = DateTime.UtcNow
            };

            await _repository.CreateAsync(message);
            return message; 
        }
    }
}
