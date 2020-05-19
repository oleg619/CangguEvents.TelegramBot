using System.Threading.Tasks;
using CangguEvents.TelegramBot.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace CangguEvents.TelegramBot.AzureFunction
{
    public class TelegramFunction
    {
        private readonly TelegramMessageHandler _telegramMessageHandler;

        public TelegramFunction(
            TelegramMessageHandler telegramMessageHandler)
        {
            _telegramMessageHandler = telegramMessageHandler;
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
            await _telegramMessageHandler.Handle(update);

            return new OkResult();
        }
    }
}