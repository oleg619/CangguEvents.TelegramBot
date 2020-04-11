using System;

namespace CangguEvents.TelegramBot.Application.Models.Commands
{
    public class EventsCommand : MessageCommandBase
    {
        public readonly bool OneDay;
        public readonly DayOfWeek DayOfWeek;

        public static EventsCommand Day(in DayOfWeek dayOfWeek, in long id)
        {
            return new EventsCommand(true, dayOfWeek, id);
        }

        public static EventsCommand WholeWeek(in long id)
        {
            return new EventsCommand(false, default, id);
        }

        private EventsCommand(in bool oneDay, in DayOfWeek dayOfWeek, in long id) : base(id)
        {
            DayOfWeek = dayOfWeek;
            OneDay = oneDay;
        }
    }
}