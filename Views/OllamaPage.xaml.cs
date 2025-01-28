using ChatBotAPI.Controllers;
using Microsoft.Maui.Controls;

namespace ChatBotAPI.Views
{
    public partial class OllamaPage : ContentPage
    {
        private readonly OllamaController _ollamaController;

        public OllamaPage()
        {
            InitializeComponent();
            _ollamaController = new OllamaController();
            MessagesCollectionView.ItemsSource = _ollamaController.Messages;
        }

        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            var userInput = UserInputEditor.Text;
            if (string.IsNullOrWhiteSpace(userInput))
            {
                await DisplayAlert("Erreur", "Veuillez entrer une question.", "OK");
                return;
            }

            try
            {
                await _ollamaController.AddUserMessage(userInput, this.Resources);

                UserInputEditor.Text = string.Empty;

                await _ollamaController.AddBotResponse(userInput, this.Resources);
            }
            catch (Exception exception)
            {
                Console.WriteLine("[ERROR] Une erreur est survenue : " + exception.Message);
                throw;
            }
            
        }
    }
}