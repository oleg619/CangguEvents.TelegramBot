using System.Reflection;
using Autofac;
using CangguEvents.MongoDb;
using CangguEvents.TelegramBot.Application.Models.Commands;
using CangguEvents.TelegramBot.Application.Services;
using CangguEvents.TelegramBot.Webhook.Configurations;
using CangguEvents.TelegramBot.Webhook.Helpers;
using CangguEvents.TelegramBot.Webhook.Middleware;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace CangguEvents.TelegramBot.Webhook
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddApplicationPart(typeof(Startup).Assembly)
                .AddNewtonsoftJson(options => options.SerializerSettings.Formatting = Formatting.Indented);

            services.AddOptions();

            services.AddInfrastructure();
            
            services.AddControllers();

            services.AddRouting(options => options.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<CorsMiddleware>();
            app.UseMiddleware<PingMiddleware>();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<SerilogMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<TelegramHostedService>()
                .As<IHostedService>()
                .InstancePerDependency();

            var config = GetBotConfiguration();
            ConfigureTelegram(builder, config);
            builder.RegisterAutomapper();
            builder.RegisterType<TelegramMessengerSender>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<TelegramMessageHandler>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<MessageParser>().AsSelf();

            builder.RegisterInstance(config).SingleInstance();
            builder.RegisterInstance(Log.Logger).AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterModule<MediatorModule>();
        }

        protected virtual void ConfigureTelegram(ContainerBuilder app, BotConfiguration configuration)
        {
            app.RegisterTelegram(configuration);
        }

        private BotConfiguration GetBotConfiguration()
        {
            var config = new BotConfiguration();

            Configuration.GetSection("BotConfiguration").Bind(config);
            return config;
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