using System;
using System.Threading;
using System.Threading.Tasks;
using CangguEvents.RabbitMq.Messages;
using CangguEvents.RabbitMq.RabbitMq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CangguEvents.TelegramBot.EventsSender
{
    public class Worker : BackgroundService
    {
        private readonly IBusPublisher _publisher;
        private readonly ILogger<Worker> _logger;

        public Worker(
            IBusPublisher publisher,
            ILogger<Worker> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var userCreated = new UserCreated(Guid.NewGuid(), "email", "firstName",
                "lastName", "address", "country");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Sending message: {@message}", userCreated);
                await _publisher.PublishAsync(userCreated, GetContext());

                await Task.Delay(1000, stoppingToken);
            }
        }

        protected ICorrelationContext GetContext(Guid? resourceId = null, string resource = "")
        {
            if (!string.IsNullOrWhiteSpace(resource))
            {
                resource = $"{resource}/{resourceId}";
            }

            return CorrelationContext.Create<int>(Guid.NewGuid(), Guid.Empty, resourceId ?? Guid.Empty,
                "origin", "traceId", "context", "connectionId", "culture", resource);
        }

        public class UserCreated : IEvent
        {
            public Guid Id { get; }
            public string Email { get; }
            public string FirstName { get; }
            public string LastName { get; }
            public string Address { get; }
            public string Country { get; }

            [JsonConstructor]
            public UserCreated(Guid id, string email, string firstName,
                string lastName, string address, string country)
            {
                Id = id;
                Email = email;
                FirstName = firstName;
                LastName = lastName;
                Address = address;
                Country = country;
            }
        }
    }
}