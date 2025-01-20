using ChatBotAPI.Models; 
using System.Collections.ObjectModel;

namespace ChatBotAPI.Controllers
{
    public class ChatController
    {
        private readonly ChatModel _chatModel;
        public ObservableCollection<Message> Messages { get; private set; }

        public ChatController()
        {
            _chatModel = new ChatModel();
            Messages = new ObservableCollection<Message>();
        }

        public async Task AddUserMessage(string userInput, ResourceDictionary resources)
        {
            Messages.Add(new Message
            {
                Text = userInput,
                BubbleStyle = resources["UserMessageBubble"] as Style,
                Alignment = LayoutOptions.End
            });
        }

        public async Task AddBotResponse(string userInput, ResourceDictionary resources)
        {
            Messages.Add(new Message
            {
                Text = "Chargement...",
                BubbleStyle = resources["BotMessageBubble"] as Style,
                Alignment = LayoutOptions.Start
            });

            var response = await _chatModel.GetResponseFromOpenAI(userInput);

            Messages.RemoveAt(Messages.Count - 1);

            Messages.Add(new Message
            {
                Text = response,
                BubbleStyle = resources["BotMessageBubble"] as Style,
                Alignment = LayoutOptions.Start
            });
        }
    }
}