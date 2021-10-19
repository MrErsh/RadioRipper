using System;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Services
{
    public interface IThreadingService
    {
        Task OnUiThread(Action action);

        Task<TResult> OnUiThreadAsync<TResult>(Func<TResult> func);
    }
}
