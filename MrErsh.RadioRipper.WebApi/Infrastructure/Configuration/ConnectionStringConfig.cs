namespace MrErsh.RadioRipper.WebApi.Configuration
{
    public class ConnectionStringsSection : ConfigSection
    {
        public ConnectionStringsSection() : base("ConnectionStrings") { }

        public string AppDbConnection { get; set; }
        public string IdentityConnection { get; set; }
        public string LogConnection { get; set; }
    }
}
