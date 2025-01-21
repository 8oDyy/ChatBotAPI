using System.Text;
using System.Text.Json;

namespace ChatBotAPI.Models
{
    public class ChatModel
    {
        // URL de l'API OpenAI pour récupérer les réponses du modèle de chat
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";

        // Clé API pour authentifier les requêtes auprès d'OpenAI
        private const string ApiKey = "Votre_Clé_API"; 

        // Méthode principale pour envoyer une requête utilisateur à OpenAI et récupérer une réponse
        public async Task<string> GetResponseFromOpenAI(string userInput)
        {
            using var client = new HttpClient(); // Client HTTP pour envoyer les requêtes
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}"); // Ajout de l'authentification à la requête

            // Préparation du corps de la requête avec le modèle utilisé et les messages du chat
            var requestBody = new
            {
                model = "gpt-3.5-turbo", // Nom du modèle OpenAI utilisé
                messages = new[]
                {
                    new { role = "system", content = "Tu es un assistant qui répond au chat" }, // Contexte initial pour le modèle
                    new { role = "user", content = userInput } // Message de l'utilisateur
                },
                max_tokens = 150 // Limite des tokens pour la réponse générée
            };

            var json = JsonSerializer.Serialize(requestBody); // Sérialisation en JSON
            var content = new StringContent(json, Encoding.UTF8, "application/json"); // Préparation de la requête HTTP

            try
            {
                Console.WriteLine("Envoi de la requête..."); // Log du début de l'envoi
                var response = await client.PostAsync(ApiUrl, content); // Envoi de la requête à l'API

                Console.WriteLine($"Statut de la réponse : {response.StatusCode}");
                if (!response.IsSuccessStatusCode) // Vérification des erreurs HTTP
                {
                    Console.WriteLine($"Erreur HTTP : {response.StatusCode}");
                    return "Une erreur s'est produite avec l'API."; // Message d'erreur utilisateur
                }

                var responseContent = await response.Content.ReadAsStringAsync(); // Lecture de la réponse brute
                Console.WriteLine($"Réponse brute : {responseContent}");

                // Désérialisation de la réponse en un objet typé
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Ignorer la casse pour les noms des propriétés
                };
                var responseData = JsonSerializer.Deserialize<OpenAIResponse>(responseContent, options);

                // Extraction et retour de la réponse principale si disponible
                if (responseData?.Choices != null && responseData.Choices.Length > 0)
                {
                    var messageContent = responseData.Choices[0].Message.Content;
                    if (!string.IsNullOrWhiteSpace(messageContent))
                    {
                        return messageContent;
                    }
                    else
                    {
                        Console.WriteLine("Le champ Content est vide."); // Log si le contenu est vide
                        return "Aucune réponse disponible.";
                    }
                }
                Console.WriteLine("Le tableau Choices est vide ou null."); // Log si aucune réponse n'est disponible
                return "Aucune réponse disponible.";
            }
            catch (Exception ex) // Gestion des exceptions pour les erreurs réseau ou de sérialisation
            {
                Console.WriteLine($"Erreur lors de la requête : {ex.Message}");
                return "Une erreur s'est produite lors de la communication avec l'API.";
            }
        }

        // Classe représentant la réponse de l'API OpenAI
        public class OpenAIResponse
        {
            public string Id { get; set; } = string.Empty; // Identifiant unique de la réponse
            public string Object { get; set; } = string.Empty; // Type d'objet retourné (e.g., "text_completion")
            public long Created { get; set; } // Timestamp de la création
            public string Model { get; set; } = string.Empty; // Modèle utilisé pour générer la réponse
            public Choice[] Choices { get; set; } = Array.Empty<Choice>(); // Liste des choix de réponse
            public Usage Usage { get; set; } = new Usage(); // Informations sur l'utilisation des tokens
        }

        // Classe représentant un choix dans la réponse
        public class Choice
        {
            public int Index { get; set; } // Index du choix dans le tableau
            public Message Message { get; set; } = new Message(); // Message contenant le texte de la réponse
            public string FinishReason { get; set; } = string.Empty; // Raison pour laquelle le modèle a arrêté de générer
        }

        // Classe représentant un message dans la conversation
        public class Message
        {
            public string Role { get; set; } = string.Empty; // Rôle de l'émetteur ("user", "system", ou "assistant")
            public string Content { get; set; } = string.Empty; // Contenu textuel du message
        }

        // Classe contenant les informations d'utilisation des tokens
        public class Usage
        {
            public int PromptTokens { get; set; } // Nombre de tokens dans l'entrée
            public int CompletionTokens { get; set; } // Nombre de tokens générés
            public int TotalTokens { get; set; } // Nombre total de tokens utilisés
        }
    }
}
