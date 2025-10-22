namespace CRM.Domain.Common.Extensions;

public static class DateTimeExtensions
{
    public static DateTime UtcToBrazilTime(this DateTime dateTimeUtc)
    {
        var brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        return TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, brazilTimeZone);
    }

    public static DateTime? UtcToBrazilTime(this DateTime? dateTimeUtc)
    {
        if(dateTimeUtc is null)
        {
            return null;
        }

        var brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        return TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc.Value, brazilTimeZone);
    }

    public static DateTime BrazilTimeToUtc(this DateTime dateTime)
    {
        var brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        var dateTimeUnspecified = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);

        return TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspecified, brazilTimeZone);
    }

    public static DateTime? BrazilTimeToUtc(this DateTime? dateTime)
    {
        if (dateTime is null)
        {
            return null;
        }

        var brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        var dateTimeUnspecified = DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Unspecified);

        return TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspecified, brazilTimeZone);
    }
}
