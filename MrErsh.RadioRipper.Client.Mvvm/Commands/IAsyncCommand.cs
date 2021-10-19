using System.Threading.Tasks;
using System.Windows.Input;

namespace MrErsh.RadioRipper.Client.Mvvm
{
    public interface IAsyncCommand : ICommand, IHasLabel
    {
        Task ExecuteAsync(object parameter);
    }

    public interface IAsyncCommand<TResult> : IAsyncCommand
    {
        new Task<TResult> ExecuteAsync(object parameter);
    }
}
