using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Mvvm
{
    public interface IAsyncResultCommand<TResult>
    {
        Task<TResult> GetResultAsync(object parameter);
    }
}
