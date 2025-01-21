using ChatBotAPI.Models; 
using System.Collections.ObjectModel;

namespace ChatBotAPI.Controllers
{
    public class ChatController
    {
        // Instance du modèle de chat pour interagir avec l'API OpenAI
        private readonly ChatModel _chatModel;

        // Collection observable des messages échangés, utilisée pour mettre à jour l'interface utilisateur en temps réel
        public ObservableCollection<Message> Messages { get; private set; }

        // Constructeur initialisant le modèle et la collection de messages
        public ChatController()
        {
            _chatModel = new ChatModel();
            Messages = new ObservableCollection<Message>();
        }

        // Ajoute un message utilisateur à la collection avec le style et l'alignement appropriés
        public async Task AddUserMessage(string userInput, ResourceDictionary resources)
        {
            Messages.Add(new Message
            {
                Text = userInput, // Texte du message saisi par l'utilisateur
                BubbleStyle = resources["UserMessageBubble"] as Style, // Style visuel de la bulle utilisateur
                Alignment = LayoutOptions.End // Alignement à droite pour les messages utilisateur
            });
        }

        // Envoie une requête à OpenAI et affiche la réponse du bot dans la collection
        public async Task AddBotResponse(string userInput, ResourceDictionary resources)
        {
            // Ajoute un message temporaire indiquant que la réponse est en cours de chargement
            Messages.Add(new Message
            {
                Text = "Chargement...", // Texte temporaire affiché pendant le traitement
                BubbleStyle = resources["BotMessageBubble"] as Style, // Style visuel de la bulle bot
                Alignment = LayoutOptions.Start // Alignement à gauche pour les messages bot
            });

            // Obtient la réponse du modèle de chat OpenAI
            var response = await _chatModel.GetResponseFromOpenAI(userInput);

            // Retire le message temporaire une fois que la réponse est disponible
            Messages.RemoveAt(Messages.Count - 1);

            // Ajoute la réponse finale du bot à la collection
            Messages.Add(new Message
            {
                Text = response, // Réponse générée par OpenAI
                BubbleStyle = resources["BotMessageBubble"] as Style, // Style visuel de la bulle bot
                Alignment = LayoutOptions.Start // Alignement à gauche pour les messages bot
            });
        }
    }
}