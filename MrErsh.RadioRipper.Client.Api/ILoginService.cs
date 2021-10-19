using MrErsh.RadioRipper.Model.Dto;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Api
{
    public interface ILoginService
    {
        Task<string> LoginAsync(LoginDto loginDto);
    }
}
