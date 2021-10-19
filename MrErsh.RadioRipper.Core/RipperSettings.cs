namespace MrErsh.RadioRipper.Core
{
    /// <param name="Interval">Ripper timer interval (s);</param>
    /// <param name="NumOfAttemtps">Number of attempts</param>
    public record RipperSettings(int Interval, int NumOfAttempts);
}
