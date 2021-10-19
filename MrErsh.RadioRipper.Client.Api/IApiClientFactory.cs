namespace MrErsh.RadioRipper.Client.Api
{
    public interface IApiClientFactory
    {
        /// <exception cref="AuthorizationException" />
        IApiClient Create();
    }
}
