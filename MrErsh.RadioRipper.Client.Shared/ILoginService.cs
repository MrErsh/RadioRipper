using MrErsh.RadioRipper.Model.Dto;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Services
{
    [Obsolete]
    public interface ICredentialProvider
    {
        Task<LoginDto> GetAsync(CancellationToken cancellationToken = default);
    }
}
