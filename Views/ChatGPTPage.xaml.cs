using ChatBotAPI.Controllers;
using Microsoft.Maui.Controls;

namespace ChatBotAPI.Views
{
    public partial class ChatGPTPage : ContentPage
    {
        private readonly ChatController _chatController;

        public ChatGPTPage()
        {
            InitializeComponent();
            _chatController = new ChatController();
            MessagesCollectionView.ItemsSource = _chatController.Messages;
        }

        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            var userInput = UserInputEditor.Text;
            if (string.IsNullOrWhiteSpace(userInput))
            {
                await DisplayAlert("Erreur", "Veuillez entrer une question.", "OK");
                return;
            }

            // Ajouter le message utilisateur via le contrôleur
            await _chatController.AddUserMessage(userInput, this.Resources);

            UserInputEditor.Text = string.Empty;

            // Ajouter la réponse du bot via le contrôleur
            await _chatController.AddBotResponse(userInput, this.Resources);
        }
    }
}