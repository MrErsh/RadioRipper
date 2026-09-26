namespace MrErsh.RadioRipper.Core
{
    public record RipperSettings(
        int Interval,
        int NumOfAttempts = 1,
        int ConnectTimeoutMs = 5000,
        int ReadTimeoutMs = 10000);
}
