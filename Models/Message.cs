using Microsoft.Maui.Controls;

namespace ChatBotAPI.Models
{
    public class Message
    {
        // Contient le texte du message échangé dans le chat (écrit par l'utilisateur ou généré par le bot)
        public string Text { get; set; }          

        // Définit le style visuel de la bulle de message (exemple : couleur ou forme différente pour le bot et l'utilisateur)
        public Style BubbleStyle { get; set; }   

        // Spécifie l'alignement de la bulle de message dans l'interface utilisateur (exemple : gauche pour le bot, droite pour l'utilisateur)
        public LayoutOptions Alignment { get; set; } 
    }
}