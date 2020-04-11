using System;

namespace CangguEvents.TelegramBot.Application.Services
{
    public static class DateTimeService
    {
        private static readonly TimeZoneInfo
            CangguTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

        public static DateTime CangguTimeNow => TimeZoneInfo.ConvertTime(DateTime.Now, CangguTimeZone);
    }
}