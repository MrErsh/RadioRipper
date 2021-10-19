namespace MrErsh.RadioRipper.WebApi.Configuration
{
    public sealed class Ripper
    {
        public const string SECTION_NAME = "Ripper";

        public int Interval { get; set; }

        public int NumOfAttempts { get; set; }
    }
}
