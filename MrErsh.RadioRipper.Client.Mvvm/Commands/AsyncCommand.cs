using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Mvvm
{
    public class AsyncCommand : AsyncCommandBase
    {
        #region Fields

        private readonly Func<Task> _command;
        private readonly Func<bool> _canExecute;

        private Task _task;

        #endregion

        #region Constructors

        public AsyncCommand(Func<Task> command, Func<bool> canExecute = null) : this(command, canExecute, null, null) {}

        public AsyncCommand(Func<Task> command,
                            Func<bool> canExecute,
                            INotifyPropertyChanged target,
                            params Expression<Func<object>>[] dependentPropertyExpressions)
            : base(target, dependentPropertyExpressions)
        {
            _command = command;
            _canExecute = canExecute;
        }

        #endregion

        #region Implemntation of ICommand

        public override bool CanExecute(object parameter)
        {
            return (_task == null || _task.IsCompleted) 
                && (_canExecute?.Invoke() ?? true);
        }

        public override async Task ExecuteAsync(object parameter)
        {
            _task = _command();
            await _task.ConfigureAwait(false);
        }

        #endregion
    }
}
