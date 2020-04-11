using System;
using System.Net.Http;
using System.Threading.Tasks;
using CangguEvents.IntegrationTests.Utils;
using NSubstitute;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;
using Xunit.Abstractions;

namespace CangguEvents.IntegrationTests
{
    // ReSharper disable once InconsistentNaming
    public class TelegramController_ShouldHandle
    {
        public TelegramController_ShouldHandle(ITestOutputHelper output)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(output)
                .CreateLogger();
        }

        [Theory]
        [Conventions]
        public async Task StartCommand(HttpClient client, ITelegramBotClient telegramBotClient)
        {
            var chatId = 321;
            var update = new Update
            {
                Message = new Message
                {
                    MessageId = 123,
                    Chat = new Chat {Id = chatId},
                    Text = "/start",
                    Date = DateTime.UtcNow
                }
            };

            var response = await client.PostAsJsonAsync("/api/Telegram/update/211212", update);

            response.EnsureSuccessStatusCode();

            await telegramBotClient.Received()
                .SendTextMessageAsync(Arg.Is<ChatId>(x => x.Identifier == chatId), "Choose", ParseMode.Markdown, false,
                    false, 0, Arg.Any<IReplyMarkup>());
        }
    }
}