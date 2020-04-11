using System.Collections.Generic;
using CangguEvents.TelegramBot.Application.Services;
using MediatR;

namespace CangguEvents.TelegramBot.Application.Models.Commands
{
    public abstract class MessageCommandBase : IRequest<IReadOnlyCollection<ITelegramResponse>>
    {
        public readonly long UserId;

        protected MessageCommandBase(long userId)
        {
            UserId = userId;
        }
    }
}