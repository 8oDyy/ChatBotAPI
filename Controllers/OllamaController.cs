using System.Text;
using System.Text.Json;
using System.Collections.ObjectModel;
using ChatBotAPI.Models;

namespace ChatBotAPI.Controllers
{
    public class OllamaController
    {
        private const string ApiUrl = "http://127.0.0.1:11434/v1/chat"; // URL locale ou de l'API Ollama
        public ObservableCollection<Message> Messages { get; private set; }

        public OllamaController()
        {
            Messages = new ObservableCollection<Message>();
        }

        public async Task AddUserMessage(string userInput, ResourceDictionary resources)
        {
            Messages.Add(new Message
            {
                Text = userInput,
                Alignment = LayoutOptions.End
            });
        }

        public async Task AddBotResponse(string userInput, ResourceDictionary resources)
        {
            Messages.Add(new Message
            {
                Text = "Chargement...",
                Alignment = LayoutOptions.Start
            });

            var response = await GetResponseFromOllama(userInput);

            Messages.RemoveAt(Messages.Count - 1);

            Messages.Add(new Message
            {
                Text = response,
                Alignment = LayoutOptions.Start
            });
        }

        private async Task<string> GetResponseFromOllama(string userInput)
        {
            using var client = new HttpClient();

            var requestBody = new
            {
                prompt = userInput // Adaptez selon la spécification d'Ollama
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                Console.WriteLine($"[DEBUG] Envoi de la requête à {ApiUrl} avec le body : {json}");
        
                var response = await client.PostAsync(ApiUrl, content);

                Console.WriteLine($"[DEBUG] Réponse reçue : {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return $"Erreur API Ollama : {response.StatusCode}";
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] Contenu de la réponse : {responseContent}");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Désérialisez la réponse pour extraire les données nécessaires
                var responseData = JsonSerializer.Deserialize<OllamaResponse>(responseContent, options);

                return responseData?.Message ?? "Aucune réponse disponible.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Erreur lors de la communication avec Ollama : {ex}");
                return $"Erreur lors de la communication avec Ollama : {ex.Message}";
            }
        }

        private class OllamaResponse
        {
            public string Message { get; set; }
        }
    }
}
