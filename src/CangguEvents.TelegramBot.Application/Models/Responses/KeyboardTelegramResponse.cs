using CangguEvents.TelegramBot.Application.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace CangguEvents.TelegramBot.Application.Models.Responses
{
    public class KeyboardTelegramResponse : ITelegramResponse
    {
        public readonly IReplyMarkup KeyboardMarkup;
        public readonly string Text;

        public KeyboardTelegramResponse(IReplyMarkup keyboardMarkup, string text)
        {
            KeyboardMarkup = keyboardMarkup;
            Text = text;
        }
    }
}