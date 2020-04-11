using CangguEvents.TelegramBot.Application.Services;

namespace CangguEvents.TelegramBot.Application.Models.Responses
{
    public class TextTelegramResponse : ITelegramResponse
    {
        public readonly string Text;

        public TextTelegramResponse(string text)
        {
            Text = text;
        }
    }
}