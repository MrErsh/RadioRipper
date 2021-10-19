using MrErsh.RadioRipper.Client.Dialogs;
using MrErsh.RadioRipper.Client.Shared;
using MrErsh.RadioRipper.Model.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Infrastructure
{
    public class DialogLoginInfoProvider : ILoginInfoProvider
    {
        private readonly IThreadingService _threadingService;

        public DialogLoginInfoProvider(IThreadingService threadingService)
            => _threadingService = threadingService;

        public async Task<LoginDto> GetAsync(CancellationToken canellationToken = default)
        {
            var loginInfo = await _threadingService
                    .OnUiThreadAsync<LoginDto>(() =>
                    {
                        var loginDialog = new LoginDialog();
                        var loginDialogResult = loginDialog.ShowDialog();
                        if (loginDialogResult != true)
                            return null;

                        var vm = loginDialog.VM;
                        return new LoginDto { Password = vm.Password, UserName = vm.Login };
                    })
                    .ConfigureAwait(false);

            return loginInfo;
        }
    }
}
