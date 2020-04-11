using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CangguEvents.TelegramBot.Application.Mediatr.Base;
using CangguEvents.TelegramBot.Application.Models.Commands;
using CangguEvents.TelegramBot.Application.Models.Responses;
using CangguEvents.TelegramBot.Application.Services;

namespace CangguEvents.TelegramBot.Application.Mediatr.Handlers
{
    public class UnknownHandler : IRequestHandlerDomain<UnknownCommand>
    {
        public async Task<IReadOnlyCollection<ITelegramResponse>> Handle(UnknownCommand request,
            CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            return new[] {new TextTelegramResponse("Unknown error")};
        }
    }
}