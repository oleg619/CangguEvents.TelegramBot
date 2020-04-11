using CangguEvents.TelegramBot.Application.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace CangguEvents.TelegramBot.Application.Models.Responses
{
    public class EditKeyboardTelegramResponse : ITelegramResponse
    {
        public readonly InlineKeyboardMarkup Keyboard;
        public readonly string Text;

        public EditKeyboardTelegramResponse(InlineKeyboardMarkup keyboard, string text)
        {
            Keyboard = keyboard;
            Text = text;
        }
    }
}