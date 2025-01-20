using ChatBotAPI.Controllers;
using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace ChatBotAPI.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly ChatController _chatController;
        public ObservableCollection<Message> Messages { get; set; }

        public MainPage()
        {
            InitializeComponent();
            _chatController = new ChatController();
            Messages = new ObservableCollection<Message>();
            MessagesCollectionView.ItemsSource = Messages;
        }

        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var userInput = UserInputEditor.Text;
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    await DisplayAlert("Erreur", "Veuillez entrer une question.", "OK");
                    return;
                }

                // Ajouter le message utilisateur à la liste
                Messages.Add(new Message
                {
                    Text = userInput,
                    BubbleStyle = this.Resources["UserMessageBubble"] as Style,
                    Alignment = LayoutOptions.End
                });

                UserInputEditor.Text = string.Empty;

                // Afficher un message de chargement
                Messages.Add(new Message
                {
                    Text = "Chargement...",
                    BubbleStyle = this.Resources["BotMessageBubble"] as Style,
                    Alignment = LayoutOptions.Start
                });

                // Obtenir la réponse du chatbot
                var response = await _chatController.GetChatbotResponse(userInput);

                // Supprimer le message de chargement
                Messages.RemoveAt(Messages.Count - 1);

                // Ajouter le message du bot à la liste
                Messages.Add(new Message
                {
                    Text = response,
                    BubbleStyle = this.Resources["BotMessageBubble"] as Style,
                    Alignment = LayoutOptions.Start
                });
            }
            catch (Exception ex)
            {
                // Ajouter un message d'erreur à la liste
                Messages.Add(new Message
                {
                    Text = "Une erreur s'est produite. Veuillez réessayer.",
                    BubbleStyle = this.Resources["BotMessageBubble"] as Style,
                    Alignment = LayoutOptions.Start
                });
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }
    }

    public class Message
    {
        public string Text { get; set; }          // Contenu du message
        public Style BubbleStyle { get; set; }   // Style de la bulle (utilisateur ou bot)
        public LayoutOptions Alignment { get; set; } // Alignement de la bulle
    }
}