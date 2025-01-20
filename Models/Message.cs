using Microsoft.Maui.Controls;

namespace ChatBotAPI.Models
{
    public class Message
    {
        public string Text { get; set; }          // Contenu du message
        public Style BubbleStyle { get; set; }   // Style de la bulle (utilisateur ou bot)
        public LayoutOptions Alignment { get; set; } // Alignement de la bulle
    }
}