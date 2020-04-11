using System;
using System.Threading.Tasks;
using CangguEvents.TelegramBot.Application.Models.Responses;

namespace CangguEvents.TelegramBot.Application.Services
{
    public interface IMessengerSender
    {
        Task SendKeyboard(KeyboardTelegramResponse response, in long chatId);
        Task SendLocation(LocationTelegramResponse response, in long chatId);
        Task SendText(TextTelegramResponse response, in long chatId);
        Task SendPhoto(PhotoTelegramResponse response, in long chatId);
        Task EditMessageText(EditKeyboardTelegramResponse response, in int messageId, in long chatId);
        Task AnswerToCallback(string callbackQueryId);

        Task Send(ITelegramResponse telegramResponse, long chatId, int messageId)
        {
            return telegramResponse switch
            {
                TextTelegramResponse text => SendText(text, chatId),
                PhotoTelegramResponse photo => SendPhoto(photo, chatId),
                LocationTelegramResponse location => SendLocation(location, chatId),
                KeyboardTelegramResponse keyboard => SendKeyboard(keyboard, chatId),
                EditKeyboardTelegramResponse response => EditMessageText(response, messageId, chatId),
                AnswerToCallback callback => AnswerToCallback(callback.CallbackId),

                _ => throw new ArgumentOutOfRangeException(nameof(telegramResponse),
                    telegramResponse.GetType().FullName, "Unknown command")
            };
        }
    }
}