using JetBrains.Annotations;
using MrErsh.RadioRipper.Client.Services;
using MrErsh.RadioRipper.Model.Dto;
using Serilog;
using System;
using Windows.Security.Credentials;

namespace MrErsh.RadioRipper.Client.Services
{
    public class CredentialStorage : ICredentialStorage
    {
        private const string RESOURCE_NAME = "RadioRipper";

        private readonly ILogger _logger;
        private readonly PasswordVault _vault = new();

        public CredentialStorage(ILogger logger)
        {
            _logger = logger;
        }

        public void Store(string userName, string password)
        {
            lock (_vault)
            {
                var credential = new PasswordCredential(RESOURCE_NAME, userName, password);
                _vault.Add(credential);
            }
        }

        [CanBeNull]
        public LoginDto GetLoginInfo()
        {
            lock (_vault)
            {
                try
                {
                    var resource = _vault.FindAllByResource(RESOURCE_NAME)[0];
                    if (resource == null)
                        return null;

                    var cred = _vault.Retrieve(RESOURCE_NAME, resource.UserName);
                    return new LoginDto(cred.UserName, cred.Password);
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Password vault access error");
                    return null;
                }
            }
        }

        public void Clear()
        {
            lock (_vault)
            {
                var all = _vault.RetrieveAll();
                foreach (var resource in all)
                    _vault.Remove(resource);
            }
        }
    }
}
