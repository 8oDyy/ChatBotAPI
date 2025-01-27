using Microsoft.Maui.Controls;
using System.Text.Json.Serialization;

namespace ChatBotAPI.Models
{
    public class Message
    {
        public string Text { get; set; }

        [JsonIgnore] // Exclut la propriété de la sérialisation JSON
        public Style BubbleStyle { get; set; }

        public LayoutOptions Alignment { get; set; }
    }

}