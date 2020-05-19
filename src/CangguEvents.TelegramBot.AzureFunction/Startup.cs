using System;
using System.Net.Http;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection.AzureFunctions;
using CangguEvents.MongoDb;
using CangguEvents.TelegramBot.Application.Models.Commands;
using CangguEvents.TelegramBot.Application.Services;
using CangguEvents.TelegramBot.AzureFunction;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

[assembly: FunctionsStartup(typeof(Startup))]

namespace CangguEvents.TelegramBot.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder
                .UseAppSettings()
                .UseAutofacServiceProviderFactory(ConfigureContainer);
        }

        private void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<IHttpClientFactory>();
            builder
                .RegisterAssemblyTypes(typeof(Startup).Assembly)
                .InNamespace("CangguEvents.TelegramBot.AzureFunction")
                .AsSelf() // Azure Functions core code resolves a function class by itself.
                .InstancePerTriggerRequest(); // This will scope nested dependencies to each function execution

            var token = GetToken();
            builder.Register<ITelegramBotClient>(provider => GetTelegramBotClient(provider, token));

            builder.RegisterType<TelegramMessengerSender>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<TelegramMessageHandler>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<MessageParser>().AsSelf();
            builder.RegisterModule<MediatorModule>();
            builder.RegisterType<HttpClient>();
            builder.AddInfrastructure();
            builder.RegisterAutomapper();
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

        private static TelegramBotClient GetTelegramBotClient(IComponentContext serviceProvider, string token)
        {
            var httpClient = serviceProvider.Resolve<HttpClient>();

            var telegramClient = new TelegramBotClient(token, httpClient);
            return telegramClient;
        }
    }

    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            // Register all the Command classes (they implement IRequestHandler) in assembly holding the Commands
            builder.RegisterAssemblyTypes(typeof(MessageCommandBase).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register all the ExceptionHandler classes (they implement IRequestExceptionHandler) in assembly holding the ExceptionHandler

            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => componentContext.TryResolve(t, out var o) ? o : null;
            });
        }
    }
}