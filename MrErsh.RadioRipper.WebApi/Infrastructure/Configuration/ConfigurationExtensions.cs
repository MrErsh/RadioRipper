using Microsoft.Extensions.Configuration;

namespace MrErsh.RadioRipper.WebApi.Configuration
{
    public static class ConfigurationExtensions
    {
        public static T GetSection<T>(this IConfiguration config, string sectionName) where T : new()
        {
            var section = new T();
            config.GetSection(sectionName).Bind(section);
            return section;
        }

        public static T GetSection<T>(this IConfiguration config) where T : ConfigSection, new()
        {
            var section = new T();
            config.GetSection(section.Path).Bind(section);
            return section;
        }
    }
}
