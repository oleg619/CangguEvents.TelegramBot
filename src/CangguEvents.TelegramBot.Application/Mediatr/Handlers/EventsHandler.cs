using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CangguEvents.Domain.Models;
using CangguEvents.Domain.Repositories;
using CangguEvents.TelegramBot.Application.Mediatr.Base;
using CangguEvents.TelegramBot.Application.Models;
using CangguEvents.TelegramBot.Application.Models.Commands;
using CangguEvents.TelegramBot.Application.Models.Responses;
using CangguEvents.TelegramBot.Application.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace CangguEvents.TelegramBot.Application.Mediatr.Handlers
{
    public class EventsHandler : IRequestHandlerDomain<EventsCommand>, IRequestHandlerDomain<FullEventInfoCommand>,
        IRequestHandlerDomain<ShortEventInfoCommand>
    {
        private readonly IEventsRepository _repository;
        private readonly IUserStateRepository _stateRepository;

        public EventsHandler(
            IEventsRepository repository,
            IUserStateRepository stateRepository)
        {
            _repository = repository;
            _stateRepository = stateRepository;
        }

        public async Task<IReadOnlyCollection<ITelegramResponse>> Handle(EventsCommand command,
            CancellationToken cancellationToken)
        {
            var eventInfos = await GetEventInfos(command);
            var userState = await _stateRepository.GetUserState(command.UserId, cancellationToken);
            return eventInfos.SelectMany(info => GetResponseForEvents(info, userState)).ToList();
        }

        public async Task<IReadOnlyCollection<ITelegramResponse>> Handle(FullEventInfoCommand request,
            CancellationToken cancellationToken)
        {
            var eventInfo = await _repository.GetEvent(request.EventId, cancellationToken);
            var inlineKeyboardButton =
                InlineKeyboardButton.WithCallbackData("Hide", $"{CommandMessages.CallbackHide}:{eventInfo.Id}");

            return new List<ITelegramResponse>
            {
                new EditKeyboardTelegramResponse(new InlineKeyboardMarkup(inlineKeyboardButton),
                    FormatCaption(eventInfo))
            };
        }

        public async Task<IReadOnlyCollection<ITelegramResponse>> Handle(ShortEventInfoCommand request,
            CancellationToken cancellationToken)
        {
            var eventInfo = await _repository.GetEvent(request.EventId, cancellationToken);

            var inlineKeyboardButton =
                InlineKeyboardButton.WithCallbackData("Show more", $"{CommandMessages.CallbackHide}:{eventInfo.Id}");

            return new List<ITelegramResponse>
            {
                new EditKeyboardTelegramResponse(new InlineKeyboardMarkup(inlineKeyboardButton), eventInfo.Name)
            };
        }

        public static string FormatCaption(EventInfo eventInfo)
        {
            var workDays = eventInfo.DayOfWeeks.GetWorkDaysAsString();

            return $"[{eventInfo.Name}]({eventInfo.Location.GoogleUrl})\n{eventInfo.Description}\n*{workDays}*";
        }
        
        private static IEnumerable<ITelegramResponse> GetResponseForEvents(EventInfo eventInfo, UserState userState)
        {
            if (userState.ShortInfo)
            {
                return ShortInfoEventResult(eventInfo);
            }

            var caption = FormatCaption(eventInfo);

            var telegramResponsePhoto = new PhotoTelegramResponse(eventInfo.Image, caption);
            var telegramResponseLocation = new LocationTelegramResponse(eventInfo.Location);

            return new List<ITelegramResponse> {telegramResponsePhoto, telegramResponseLocation};
        }

        private static IEnumerable<ITelegramResponse> ShortInfoEventResult(EventInfo eventInfo)
        {
            var inlineKeyboardMarkup =
                new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Show more", $"full:{eventInfo.Id}"));
            return new[] {new KeyboardTelegramResponse(inlineKeyboardMarkup, eventInfo.Name)};
        }
        
        private Task<List<EventInfo>> GetEventInfos(EventsCommand command)
        {
            return command.OneDay ? _repository.GetEvents(command.DayOfWeek) : _repository.GetAllEvents();
        }
    }
}