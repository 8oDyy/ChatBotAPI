using ChatBotAPI.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.IO;
using Microsoft.Maui.Controls;

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

            _ = LoadChatHistory();
        }

        private string GetHistoryFilePath()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = Path.Combine(folder, "ChatBotAPI", "chat_history.json");

            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            return filePath;
        }

        private async Task SaveMessageToHistory(Message message)
        {
            try
            {
                var historyFilePath = GetHistoryFilePath();

                var historyMessage = new
                {
                    Text = message.Text,
                    Alignment = message.Alignment.ToString()
                };

                string json = JsonSerializer.Serialize(historyMessage);

                await File.AppendAllTextAsync(historyFilePath, json + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sauvegarde de l'historique : {ex.Message}");
            }
        }

        public async Task LoadChatHistory()
        {
            try
            {
                var historyFilePath = GetHistoryFilePath();

                if (File.Exists(historyFilePath))
                {
                    var lines = await File.ReadAllLinesAsync(historyFilePath);
                    foreach (var line in lines)
                    {
                        var historyMessage = JsonSerializer.Deserialize<HistoryMessage>(line);

                        var alignment = historyMessage.Alignment switch
                        {
                            "Start" => LayoutOptions.Start,
                            "End" => LayoutOptions.End,
                            _ => LayoutOptions.Fill
                        };

                        Messages.Add(new Message
                        {
                            Text = historyMessage.Text,
                            Alignment = alignment
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement de l'historique : {ex.Message}");
            }
        }

        public async Task AddUserMessage(string userInput, ResourceDictionary resources)
        {
            var userMessage = new Message
            {
                Text = userInput,
                BubbleStyle = resources["UserMessageBubble"] as Style,
                Alignment = LayoutOptions.End
            };

            Messages.Add(userMessage);
            await SaveMessageToHistory(userMessage);
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

            var botMessage = new Message
            {
                Text = response,
                BubbleStyle = resources["BotMessageBubble"] as Style,
                Alignment = LayoutOptions.Start
            };

            Messages.Add(botMessage);
            await SaveMessageToHistory(botMessage);
        }

        private class HistoryMessage
        {
            public string Text { get; set; }
            public string Alignment { get; set; }
        }
    }
}