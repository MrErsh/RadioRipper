namespace MrErsh.RadioRipper.Client.Services
{
    public interface IAppSettings
    {
        public string Host { get; set; }

        public string ActualApiHost { get; }
    }
}
