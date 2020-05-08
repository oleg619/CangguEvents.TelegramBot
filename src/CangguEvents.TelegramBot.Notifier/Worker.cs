using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CangguEvents.Domain.Models;
using CangguEvents.Domain.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CangguEvents.TelegramBot.Notifier
{
    public class Worker : BackgroundService
    {
        private readonly DateTimeService _dateTimeService;
        private readonly IEventsRepository _eventsRepository;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IUserStateRepository _userStateRepository;

        public Worker(
            IUserStateRepository userStateRepository,
            DateTimeService dateTimeService,
            IEventsRepository eventsRepository,
            ITelegramBotClient telegramBotClient
        )
        {
            _dateTimeService = dateTimeService;
            _eventsRepository = eventsRepository;
            _telegramBotClient = telegramBotClient;
            _userStateRepository = userStateRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _dateTimeService.WaitToTomorrow(stoppingToken);
                var userStates = await _userStateRepository.GetSubscribedUsers(stoppingToken);
                var eventInfos = await _eventsRepository.GetEvents(DateTime.UtcNow.DayOfWeek, stoppingToken);
                if (eventInfos.Count == 0)
                {
                    return;
                }

                var notifies = GetNotifies(userStates, eventInfos);
                await SendNotifies(stoppingToken, notifies);
            }
        }

        private async Task SendNotifies(CancellationToken stoppingToken,
            IEnumerable<(long Id, string Message)> notifies)
        {
            foreach (var (id, message) in notifies)
            {
                await _telegramBotClient.SendTextMessageAsync(id, message, ParseMode.Markdown,
                    cancellationToken: stoppingToken);
            }
        }

        private IEnumerable<(long Id, string Message)> GetNotifies(List<UserState> userStates,
            List<EventInfo> eventInfos) => userStates.Select(userState => new
            {
                userState, message = SelectMessage(eventInfos)
            })
            .Select(t => (t.userState.Id, t.message));

        private static string SelectMessage(List<EventInfo> eventInfos)
        {
            return eventInfos.Aggregate("Today excellent events :",
                (current, eventInfo) => current + $"[{eventInfo.Name}]({eventInfo.Location.GoogleUrl}\n");
        }
    }

    public class DateTimeService
    {
        private readonly ILogger<DateTimeService> _logger;


        public DateTimeService(ILogger<DateTimeService> logger)
        {
            _logger = logger;
        }

        public async Task WaitToTomorrow(CancellationToken token = default)
        {
            var timeToTomorrow = GetTimeToTomorrow();
            _logger.LogInformation("Wait {time}", timeToTomorrow);
            await Task.Delay(timeToTomorrow, token);
        }

        private static TimeSpan GetTimeToTomorrow()
        {
            var now = DateTime.Now;
            var tomorrow = DateTime.Today.AddDays(1);

            return tomorrow - now;
        }
    }
}