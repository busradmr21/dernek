namespace SigortaDernegi.Utilities;

public static class TurkeyDateTime
{
    private static readonly TimeZoneInfo TurkeyTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows()
                ? "Turkey Standard Time"
                : "Europe/Istanbul");

    public static DateTime FromUtc(DateTime value)
    {
        var utcValue = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        return TimeZoneInfo.ConvertTimeFromUtc(
            utcValue,
            TurkeyTimeZone);
    }
}
