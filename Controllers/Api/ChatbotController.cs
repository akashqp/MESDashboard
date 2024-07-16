using Microsoft.AspNetCore.Mvc;
using MESDashboard.Services;
using Microsoft.AspNetCore.Http;

namespace MESDashboard.Controllers.Api
{
    [Route("api/chatbot")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly ChatbotService _chatbotService;

        public ChatbotController(ChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        // Post Request to get the Chatbot Response
        [HttpPost]
        public IActionResult Post([FromBody] ChatMessage message)
        {
            var response = _chatbotService.GetResponseAsync(message.Message);
            return Ok(new { message = response });
        }

        // Get Request to get the Chat History
        [HttpGet]
        public IActionResult Get()
        {
            var chatHistory = _chatbotService.GetChatHistory();
            return Ok(chatHistory);
        } 
    } 

    [Route("api/chathistory")]
    [ApiController]
    public class ChatHistoryController : ControllerBase
    {
        private readonly ChatbotService _chatbotService;

        public ChatHistoryController(ChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        // Get Request to clear the Chat History
        [HttpGet]
        public IActionResult Get()
        {
            _chatbotService.ClearChatHistory();
            return Ok();
        }

        // Get Request to add initial message to the Chat History
        [HttpGet("addinitialmessage")]
        public IActionResult GetAddInitialMessage()
        {
            _chatbotService.AddInitialMessage();
            return Ok();
        }
    }

    // [Route("api/chatmessage")]
    // [ApiController]
    // public class ChatMessageController : ControllerBase
    // {
    //     private readonly ChatbotService _chatbotService;

    //     public ChatMessageController(ChatbotService chatbotService)
    //     {
    //         _chatbotService = chatbotService;
    //     }
    // }

    public class ChatMessage
    {
        public string Message { get; set; }
    }
}
