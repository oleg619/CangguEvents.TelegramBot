using CangguEvents.TelegramBot.Application.Services;

namespace CangguEvents.TelegramBot.Application.Models.Responses
{
    public class PhotoTelegramResponse : ITelegramResponse
    {
        public readonly byte[] Image;

        public readonly string Caption;

        public PhotoTelegramResponse(byte[] image, string caption)
        {
            Image = image;
            Caption = caption;
        }
    }
}