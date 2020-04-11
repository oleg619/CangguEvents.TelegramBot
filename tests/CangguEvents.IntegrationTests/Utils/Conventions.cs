using Autofac.Extensions.DependencyInjection;
using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace CangguEvents.IntegrationTests.Utils
{
    public class Conventions : AutoDataAttribute
    {
        public Conventions() : base(Create)
        {
        }

        private static IFixture Create()
        {
            var fixture = new Fixture();
            var builder = Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<CustomStartup>().UseTestServer(); })
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", false, false);
                })
                ;

            var testHost = builder.Build();
            var telegramBotClient = testHost.Services.GetService<ITelegramBotClient>();

            testHost.Start();
            var client = testHost.GetTestClient();

            fixture.Inject(telegramBotClient);
            fixture.Inject(client);

            return fixture;
        }
    }
}