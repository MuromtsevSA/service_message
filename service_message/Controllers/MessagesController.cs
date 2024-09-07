using service_message.Dto;
using service_message.HubMessage; 
using service_message.Models;
using service_message.Repository;
using service_message.Service;
using Microsoft.AspNetCore.Mvc;

namespace service_message.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessagesController> _logger;
        private readonly WebSocketServer _webSocketServer;

        public MessagesController(IMessageService messageService, WebSocketServer webSocketServer, ILogger<MessagesController> logger)
        {
            _messageService = messageService;
            _webSocketServer = webSocketServer;
            _logger = logger;
        }

        [HttpGet("GetRecentMessages")]
        public async Task<ActionResult<IEnumerable<Message>>> GetRecentMessages()
        {
            try
            {
                _logger.LogInformation("Fetching recent messages");
                var messages = await _messageService.GetRecentMessages();

                if (messages == null || !messages.Any())
                {
                    _logger.LogInformation("No recent messages found");
                    return NoContent();
                }

                _logger.LogInformation("Successfully fetched recent messages");
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching recent messages");
                return StatusCode(500, "Internal server error");
            }
        }

      
        [HttpPost("CreateMessage")]
        public async Task<ActionResult> CreateMessage([FromBody] MessageDto messageDto)
        {
            try
            {
                _logger.LogInformation("Creating a new message");

   
                var message = await _messageService.CreateMessage(messageDto);

                _logger.LogInformation("Successfully created a new message");

                var msgData = $"{{\"text\":\"{message.Text}\", \"order\":{message.Order}, \"time\":\"{message.Time:o}\"}}";
                _webSocketServer.BroadcastMessage(msgData);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating message");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
