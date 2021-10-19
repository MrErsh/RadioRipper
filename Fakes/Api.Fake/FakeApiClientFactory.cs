namespace MrErsh.RadioRipper.Client.Api.Fake
{
    public class FakeApiClientFactory : IApiClientFactory
    {
        public IApiClient Create() => new FakeApiClient();
    }
}
