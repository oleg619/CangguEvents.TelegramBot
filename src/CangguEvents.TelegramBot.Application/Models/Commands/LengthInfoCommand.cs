namespace CangguEvents.TelegramBot.Application.Models.Commands
{
    public class LengthInfoCommand : MessageCommandBase
    {
        public bool ShortInfo { get; }

        public LengthInfoCommand(bool shortInfo, in long userId) : base(userId)
        {
            ShortInfo = shortInfo;
        }
    }

    public class FullEventInfoCommand : MessageCommandBase
    {
        public int EventId { get; }

        public FullEventInfoCommand(int eventId, long userId) : base(userId)
        {
            EventId = eventId;
        }
    }

    public class ShortEventInfoCommand : MessageCommandBase
    {
        public int EventId { get; }

        public ShortEventInfoCommand(int eventId, long userId) : base(userId)
        {
            EventId = eventId;
        }
    }
}