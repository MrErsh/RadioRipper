using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MrErsh.RadioRipper.Client.Services
{
    public interface IDialogService
    {
        Task<(bool, TViewModel)> ShowDialogAsync<TViewModel>(
            Type dialogType,
            Func<TViewModel, ICommand> primaryCommandFactory = null,
            Func<TViewModel, ICommand> secondaryCommandFactory = null,
            Action<TViewModel> vmInitializator = null)
            where TViewModel : class;
    }
}
