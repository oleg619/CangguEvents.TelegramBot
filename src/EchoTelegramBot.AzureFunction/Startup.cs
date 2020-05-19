using System;
using System.Net.Http;
using EchoTelegramBot.AzureFunction;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

[assembly: FunctionsStartup(typeof(Startup))]

namespace EchoTelegramBot.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            var token = GetToken();

            builder.Services.AddTransient<ITelegramBotClient>(provider => GetTelegramBotClient(provider, token));
        }

        private static string GetToken()
        {
            var token = Environment.GetEnvironmentVariable("token", EnvironmentVariableTarget.Process);

            if (token is null)
            {
                throw new ArgumentException("Can not get token. Set token in environment setting");
            }

            return token;
        }

        private static TelegramBotClient GetTelegramBotClient(IServiceProvider serviceProvider, string? token)
        {
            var httpClient = serviceProvider.GetService<HttpClient>();

            var telegramClient = new TelegramBotClient(token, httpClient);
            return telegramClient;
        }
    }
}