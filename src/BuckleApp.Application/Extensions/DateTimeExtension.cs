namespace BuckleApp.Application.Extensions;

public static class DateTimeExtension
{
    public static DateTime ConvertToTimeZone(this DateTime utcDateTime, string timeZoneId)
    {
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);
    }
}