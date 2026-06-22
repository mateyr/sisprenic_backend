namespace Sisprenic.Api.Common;

// Nicaragua no observa horario de verano, por lo que el offset es fijo.
public static class BusinessClock
{
    private static readonly TimeSpan NicaraguaOffset = TimeSpan.FromHours(-6);

    public static DateOnly Today() => DateOnly.FromDateTime(DateTime.UtcNow + NicaraguaOffset);
}
