using System.Collections.Generic;
using System.Threading.Tasks;
using CangguEvents.TelegramBot.Application.Models.Responses;
using MediatR;
using Serilog;
using Telegram.Bot.Types;

namespace CangguEvents.TelegramBot.Application.Services
{
    public class TelegramMessageHandler
    {
        private readonly MessageParser _messageParser;
        private readonly IMediator _mediator;
        private readonly IMessengerSender _sender;
        private readonly ILogger _logger;

        public TelegramMessageHandler(
            MessageParser messageParser,
            IMediator mediator,
            IMessengerSender sender,
            ILogger logger)
        {
            _messageParser = messageParser;
            _sender = sender;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(Update update)
        {
            var (command, (userId, messageId, callbackQueryId)) = _messageParser.ParseMessage(update);

            _logger.Information("Parse {message} to {command}", update, command);
            var responses = await _mediator.Send(command);

            foreach (var telegramResponse in EnumerateTelegramResponses(responses, callbackQueryId))
            {
                _logger.Information("Send {message}", telegramResponse);
                await _sender.Send(telegramResponse, userId, messageId);
            }
        }

        private static IEnumerable<ITelegramResponse> EnumerateTelegramResponses(
            IReadOnlyCollection<ITelegramResponse> responses, string? callbackQueryId)
        {
            foreach (var response in responses)
            {
                yield return response;
            }

            if (callbackQueryId != null && responses.Count > 0)
            {
                yield return new AnswerToCallback(callbackQueryId);
            }
        }
    }
}