using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EchoTelegramBot.AzureFunction
{
    public class TelegramFunction
    {
        private readonly ITelegramBotClient _client;

        public TelegramFunction(ITelegramBotClient client)
        {
            _client = client;
        }

        [FunctionName("Telegram")]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest request,
            ILogger logger)
        {
            logger.LogInformation("Invoke telegram update function");

            var body = await request.ReadAsStringAsync();
            var update = JsonConvert.DeserializeObject<Update>(body);
            await HandleUpdate(update);

            return new OkResult();
        }

        private async Task HandleUpdate(Update update)
        {

            if (update.Type == UpdateType.Message)
            {
                await _client.SendTextMessageAsync(update.Message.Chat, $"Echo : {update.Message.Text}");
            }
        }
    }
}