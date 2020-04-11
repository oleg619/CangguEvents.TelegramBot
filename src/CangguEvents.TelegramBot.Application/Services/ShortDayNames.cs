using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CangguEvents.TelegramBot.Application.Services
{
    public static class ShortDayNames
    {
        private static readonly DateTimeFormatInfo Culture = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;

        public static string Get(DayOfWeek dayOfWeek)
        {
            var shortName = Culture.AbbreviatedDayNames[(int) dayOfWeek];
            return shortName;
        }

        public static string GetWorkDaysAsString(this List<DayOfWeek> dayOfWeeks)
        {
            var works = dayOfWeeks.Count switch
            {
                7 => "Works whole week",
                2 when dayOfWeeks.Contains(DayOfWeek.Sunday) &&
                       dayOfWeeks.Contains(DayOfWeek.Saturday) => "Works at weekend",
                _ => $"Works on : {string.Join(" | ", dayOfWeeks.Select(Get))}"
            };

            return works;
        }
    }
}