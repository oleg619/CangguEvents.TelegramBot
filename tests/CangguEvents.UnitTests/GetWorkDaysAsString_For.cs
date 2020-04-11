using System;
using System.Collections.Generic;
using System.Linq;
using CangguEvents.TelegramBot.Application.Services;
using FluentAssertions;
using Xunit;

namespace CangguEvents.UnitTests
{
    // ReSharper disable once InconsistentNaming
    public class GetWorkDaysAsString_For
    {
        [Fact]
        public void OneDay()
        {
            var dayOfWeeks = new List<DayOfWeek>
            {
                DayOfWeek.Friday
            };

            CheckWorkDays(dayOfWeeks, "Works on : Fri");
        }

        [Fact]
        public void TwoDays()
        {
            var dayOfWeeks = new List<DayOfWeek>
            {
                DayOfWeek.Monday, DayOfWeek.Wednesday
            };

            CheckWorkDays(dayOfWeeks, "Works on : Mon | Wed");
        }

        [Fact]
        public void WholeWeak()
        {
            var dayOfWeeks = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();

            CheckWorkDays(dayOfWeeks, "Works whole week");
        }

        [Fact]
        public void WorksAtWeekend()
        {
            var dayOfWeeks = new List<DayOfWeek>
            {
                DayOfWeek.Saturday, DayOfWeek.Sunday
            };

            CheckWorkDays(dayOfWeeks, "Works at weekend");
        }

        private static void CheckWorkDays(List<DayOfWeek> dayOfWeeks, string result)
        {
            var workDays = dayOfWeeks.GetWorkDaysAsString();

            workDays.Should().Be(result);
        }
    }
}