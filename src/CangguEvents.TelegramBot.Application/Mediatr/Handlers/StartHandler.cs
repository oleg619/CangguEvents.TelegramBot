using System.Collections.Generic;
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
    public class StartHandler : IRequestHandlerDomain<StartCommand>, IRequestHandlerDomain<BackCommand>
    {
        private readonly IUserStateRepository _stateRepository;

        public StartHandler(IUserStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }

        public async Task<IReadOnlyCollection<ITelegramResponse>> Handle(StartCommand request,
            CancellationToken cancellationToken)
        {
            var userState = new UserState(true, true);

            await _stateRepository.CreateUser(request.UserId, userState, cancellationToken);

            return StartKeyboard();
        }

        public Task<IReadOnlyCollection<ITelegramResponse>> Handle(BackCommand request,
            CancellationToken cancellationToken)
        {
            var startKeyboard = StartKeyboard();
            return Task.FromResult(startKeyboard);
        }

        private static IReadOnlyCollection<ITelegramResponse> StartKeyboard()
        {
            ReplyKeyboardMarkup replyKeyboard = new[]
            {
                new[] {CommandMessages.Today, CommandMessages.WholeWeek, CommandMessages.OneDay},
                new[] {CommandMessages.Settings},
            };
            replyKeyboard.ResizeKeyboard = true;

            return new[] {new KeyboardTelegramResponse(replyKeyboard, "Choose")};
        }
    }
}