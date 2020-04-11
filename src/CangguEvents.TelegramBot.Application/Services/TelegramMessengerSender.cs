using System.IO;
using System.Threading.Tasks;
using CangguEvents.TelegramBot.Application.Models.Responses;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace CangguEvents.TelegramBot.Application.Services
{
    public class TelegramMessengerSender : IMessengerSender
    {
        private readonly ITelegramBotClient _telegramService;
        private readonly ILogger _logger;
        private const ParseMode ParseModeMessage = ParseMode.Markdown;

        public TelegramMessengerSender(ITelegramBotClient telegramService, ILogger logger)
        {
            _telegramService = telegramService;
            _logger = logger;
        }

        public Task SendKeyboard(KeyboardTelegramResponse response, in long chatId)
        {
            return _telegramService.SendTextMessageAsync(chatId, response.Text, ParseModeMessage,
                replyMarkup: response.KeyboardMarkup);
        }

        public Task SendLocation(LocationTelegramResponse response, in long chatId)
        {
            var location = response.Location;
            return _telegramService.SendLocationAsync(chatId, location.Latitude, location.Longitude);
        }

        public Task SendText(TextTelegramResponse response, in long chatId)
        {
            return _telegramService.SendTextMessageAsync(chatId, response.Text, ParseModeMessage);
        }

        public  Task SendPhoto(PhotoTelegramResponse response, in long chatId)
        {
            using var memoryStream = new MemoryStream(response.Image);
            var photoStream = new InputOnlineFile(memoryStream);
            return _telegramService.SendPhotoAsync(chatId, photoStream, response.Caption, ParseModeMessage);
        }

        public Task EditMessageText(EditKeyboardTelegramResponse keyboard, in int messageId, in long chatId)
        {
            return _telegramService.EditMessageTextAsync(chatId, messageId, keyboard.Text, ParseModeMessage,
                replyMarkup: keyboard.Keyboard);
        }
        
        public async Task AnswerToCallback(string callbackQueryId)
        {
            try
            {
                await _telegramService.AnswerCallbackQueryAsync(callbackQueryId);
            }
            catch (InvalidParameterException e)
            {
                _logger.Warning(e, "Should to do it");
            }
        }
    }
}