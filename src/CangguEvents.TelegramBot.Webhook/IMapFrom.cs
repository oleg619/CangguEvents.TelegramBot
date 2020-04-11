using AutoMapper;

namespace CangguEvents.TelegramBot.Webhook
{
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}