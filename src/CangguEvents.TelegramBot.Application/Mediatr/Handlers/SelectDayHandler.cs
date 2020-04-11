using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CangguEvents.TelegramBot.Application.Mediatr.Base;
using CangguEvents.TelegramBot.Application.Models;
using CangguEvents.TelegramBot.Application.Models.Commands;
using CangguEvents.TelegramBot.Application.Models.Responses;
using CangguEvents.TelegramBot.Application.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace CangguEvents.TelegramBot.Application.Mediatr.Handlers
{
    public class SelectDayHandler : IRequestHandlerDomain<SelectDayCommand>
    {
        public async Task<IReadOnlyCollection<ITelegramResponse>> Handle(SelectDayCommand request,
            CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var buttons = ((DayOfWeek[]) Enum.GetValues(typeof(DayOfWeek)))
                .Select(SelectInlineButton)
                .ToList();

            return new[] {new KeyboardTelegramResponse(new InlineKeyboardMarkup(buttons), "Select day")};
        }

        private static InlineKeyboardButton SelectInlineButton(DayOfWeek dayOfWeek)
        {
            return InlineKeyboardButton.WithCallbackData(ShortDayNames.Get(dayOfWeek),
                $"{CommandMessages.CallbackDay}:{(int) dayOfWeek}");
        }
    }
}