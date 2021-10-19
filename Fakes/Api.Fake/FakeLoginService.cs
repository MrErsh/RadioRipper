using MrErsh.RadioRipper.Model.Dto;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Api.Fake
{
    public class FakeLoginService : ILoginService
    {
        public Task<string> LoginAsync(LoginDto loginDto)
            => Task.FromResult("sdflhf874b4lkgh24g974ghlekhl1i3hgl1gh993lihg");
    }
}
