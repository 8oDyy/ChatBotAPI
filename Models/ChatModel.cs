using System.Text;
using System.Text.Json;


namespace ChatBotAPI.Models
{
    public class ChatModel
    {
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private const string ApiKey ="VOTRE_CLÉ_API"; 

        public async Task<string> GetResponseFromOpenAI(string userInput)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
            {
            new { role = "system", content = "Tu es un assistant qui repond au chat" },
            new { role = "user", content = userInput }
            },
            max_tokens = 150
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                Console.WriteLine("Envoi de la requête...");
                var response = await client.PostAsync(ApiUrl, content);

                Console.WriteLine($"Statut de la réponse : {response.StatusCode}");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Erreur HTTP : {response.StatusCode}");
                    return "Une erreur s'est produite avec l'API.";
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Réponse brute : {responseContent}");

                // Désérialisation avec options
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var responseData = JsonSerializer.Deserialize<OpenAIResponse>(responseContent, options);
                
                if (responseData?.Choices != null && responseData.Choices.Length > 0)
                {
                    var messageContent = responseData.Choices[0].Message.Content;
                    if (!string.IsNullOrWhiteSpace(messageContent))
                    {
                        return messageContent;
                    }
                    else
                    {
                        Console.WriteLine("Le champ Content est vide.");
                        return "Aucune réponse disponible.";
                    }
                }
                Console.WriteLine("Le tableau Choices est vide ou null.");
                return "Aucune réponse disponible.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la requête : {ex.Message}");
                return "Une erreur s'est produite lors de la communication avec l'API.";
            }
        }

        public class OpenAIResponse
        {
            public string Id { get; set; } = string.Empty;
            public string Object { get; set; } = string.Empty;
            public long Created { get; set; }
            public string Model { get; set; } = string.Empty;
            public Choice[] Choices { get; set; } = Array.Empty<Choice>();
            public Usage Usage { get; set; } = new Usage();
        }

        public class Choice

        {
            public int Index { get; set; }
            public Message Message { get; set; } = new Message();
            public string FinishReason { get; set; } = string.Empty;
        }

        public class Message
        {
            public string Role { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
        }

        public class Usage
        {
            public int PromptTokens { get; set; }
            public int CompletionTokens { get; set; }
            public int TotalTokens { get; set; }
        }
    }
}
