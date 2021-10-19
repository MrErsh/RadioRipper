namespace MrErsh.RadioRipper.WebApi.Configuration
{
    public abstract class ConfigSection
    {
        public ConfigSection(string section) { Path = section; }

        public string Path { get; set; }
    }
}
