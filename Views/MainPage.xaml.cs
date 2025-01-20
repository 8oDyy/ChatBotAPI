using ChatBotAPI.Controllers;
using Microsoft.Maui.Controls;

namespace ChatBotAPI.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly ChatController _chatController;

        public MainPage()
        {
            InitializeComponent();
            _chatController = new ChatController();
        }

        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("Début du traitement");

                var userInput = UserInputEditor.Text;
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("Entrée utilisateur vide");
                    await DisplayAlert("Erreur", "Veuillez entrer une question.", "OK");
                    return;
                }

                Console.WriteLine("Requête en cours...");
                ChatbotResponseLabel.Text = "Chargement...";

                var response = await _chatController.GetChatbotResponse(userInput);

                Console.WriteLine($"Réponse obtenue : {response}");
                ChatbotResponseLabel.Text = $"Réponse : {response}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
                ChatbotResponseLabel.Text = "Une erreur s'est produite.";
            }
        }
    }
}