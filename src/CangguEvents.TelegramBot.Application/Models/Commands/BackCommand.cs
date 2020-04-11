namespace CangguEvents.TelegramBot.Application.Models.Commands
{
    public class BackCommand : MessageCommandBase
    {
        public BackCommand(in long userId) : base(userId)
        {
        }
    }

    public class SelectDayCommand : MessageCommandBase
    {
        public SelectDayCommand(long userId) : base(userId)
        {
        }
    }
}