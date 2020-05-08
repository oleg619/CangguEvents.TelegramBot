using System;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CangguEvents.TelegramBot.Notifier
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Log.Information("Starting up");

                await CreateHostBuilder(args)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .Build()
                    .RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(ConfigureWebHost);

        private static void ConfigureWebHost(IWebHostBuilder webBuilder)
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseUrls("http://localhost:8443");
        }
    }
}