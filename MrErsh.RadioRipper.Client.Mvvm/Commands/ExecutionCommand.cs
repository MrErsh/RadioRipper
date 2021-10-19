using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Mvvm
{
    public class ExecutionCommand<TResult> : AsyncExecutionCommandBase<TResult>
    {
        private readonly Func<Task<TResult>> _command;

        public ExecutionCommand([NotNull] Func<Task<TResult>> command,
                                [CanBeNull] Func<bool> canExecute = null,
                                [CanBeNull] INotifyPropertyChanged target = null,
                                params Expression<Func<object>>[] dependentPropertyExpressions)
            : base(target, canExecute, dependentPropertyExpressions)
        {
            _command = command;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            var task = _command();
            Execution = new NotifyTaskCompletion<TResult>(task);
            RaiseCanExecuteChanged();
            if (Execution.TaskCompletion != null)
                await Execution.TaskCompletion;
            RaiseCanExecuteChanged();
        }
    }

    public class ExecutionCommand<T, TResult> : AsyncExecutionCommandBase<TResult>
    {
        private readonly Func<T, Task<TResult>> _command;

        public ExecutionCommand([NotNull] Func<T, Task<TResult>> command,
                                [CanBeNull] Func<bool> canExecute = null,
                                [CanBeNull] INotifyPropertyChanged target = null,
                                [CanBeNull] Expression<Func<object>>[] dependentPropertyExpressions = null)
            : base(target, canExecute, dependentPropertyExpressions)
        {
            _command = command;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            T param = default;
            try
            {
                param = (T)parameter;
            }
            catch { };

            var task = _command(param);
            Execution = new NotifyTaskCompletion<TResult>(task);
            RaiseCanExecuteChanged();
            if (Execution.TaskCompletion != null)
                await Execution.TaskCompletion;
            RaiseCanExecuteChanged();
        }
    }
}
