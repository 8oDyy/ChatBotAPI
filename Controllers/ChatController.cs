using ChatBotAPI.Models;
using System.Threading.Tasks;

namespace ChatBotAPI.Controllers
{
    public class ChatController
    {
        private readonly ChatModel _chatModel;

        public ChatController()
        {
            _chatModel = new ChatModel();
        }

        public async Task<string> GetChatbotResponse(string userInput)
        {
            return await _chatModel.GetResponseFromOpenAI(userInput);
        }
    }
}