using System.Threading;
using System.Threading.Tasks;
using CangguEvents.TelegramBot.Application;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot;

namespace CangguEvents.TelegramBot.Webhook
{
    public sealed class TelegramHostedService : IHostedService
    {
        private readonly ITelegramBotClient _client;
        private readonly BotConfiguration _configuration;
        private readonly ILogger _logger;

        public TelegramHostedService(ITelegramBotClient client, BotConfiguration configuration, ILogger logger)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.DeleteWebhookAsync(cancellationToken);
            _logger.Information("Start webhook with {url} {token}", _configuration.WebhookUrl, _configuration.BotToken);
            await _client.SetWebhookAsync($"{_configuration.WebhookUrl}/{_configuration.BotToken}",
                cancellationToken: cancellationToken);
            _logger.Information("End set webhook");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _client.DeleteWebhookAsync(cancellationToken);
        }
    }
}