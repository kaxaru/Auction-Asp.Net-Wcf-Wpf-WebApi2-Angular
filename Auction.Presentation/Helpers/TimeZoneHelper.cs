using System;
using System.Linq;

namespace Auction.Presentation.Helpers
{
    public class TimeZoneHelper
    {
        public static TimeSpan ConverTimeToServer(string idTimeZone)
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            TimeSpan userOffset = timeZones.FirstOrDefault(t => t.Id == idTimeZone).BaseUtcOffset;
            TimeSpan serverOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            return userOffset - serverOffset;
        }
    }
}