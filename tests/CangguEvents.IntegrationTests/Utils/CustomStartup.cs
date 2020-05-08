using Autofac;
using CangguEvents.TelegramBot.Webhook;
using CangguEvents.TelegramBot.Webhook.Configurations;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Telegram.Bot;

namespace CangguEvents.IntegrationTests.Utils
{
    public class CustomStartup : Startup
    {
        public CustomStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureTelegram(ContainerBuilder app, BotConfiguration configuration)
        {
            var telegramBotClient = Substitute.For<ITelegramBotClient>();
            app.RegisterInstance(telegramBotClient).AsImplementedInterfaces().SingleInstance();
        }
    }
}