using System.Collections.Generic;
using CangguEvents.TelegramBot.Application.Models.Commands;
using CangguEvents.TelegramBot.Application.Services;
using MediatR;

namespace CangguEvents.TelegramBot.Application.Mediatr.Base
{
    public interface IRequestHandlerDomain<in TRequest> : IRequestHandler<TRequest, IReadOnlyCollection<ITelegramResponse>>
        where TRequest : MessageCommandBase
    {
    }
}