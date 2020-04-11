using System;
using System.Threading.Tasks;
using CangguEvents.TelegramBot.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Telegram.Bot.Types;

namespace CangguEvents.TelegramBot.Webhook.Controllers
{
    [Route("api/[controller]")]
    public class TelegramController : Controller
    {
        private readonly TelegramMessageHandler _telegramMessageHandler;
        private readonly ILogger _logger;

        public TelegramController(
            TelegramMessageHandler telegramMessageHandler,
            ILogger logger)
        {
            _telegramMessageHandler = telegramMessageHandler;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var cangguTimeNow = DateTimeService.CangguTimeNow;
            return Ok($"Hello time in canggu {cangguTimeNow}{Environment.NewLine}" +
                      "Mimi1");
        }

        [HttpPost("update/{token}")]
        public async Task Update([FromRoute] string token, [FromBody] Update update)
        {
            _logger.Information("We received new {@message}", update);
            await _telegramMessageHandler.Handle(update);
        }
    }
}