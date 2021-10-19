namespace MrErsh.RadioRipper.WebApi.Configuration
{
    public sealed class SecurityJwtOptions
    {
        public const string SECTION = "Security:Jwt";

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SigningKey { get; set; }

        public int SessionLifetimeMin { get; set; }
    }
}
