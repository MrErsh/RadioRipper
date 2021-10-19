using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Mvvm
{
    public abstract class AsyncExecutionCommandBase<TResult> : AsyncCommandBase, INotifyPropertyChanged, IAsyncResultCommand<TResult>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private NotifyTaskCompletion<TResult> _execution;
        private readonly Func<bool> _canExecute;

        protected AsyncExecutionCommandBase([CanBeNull] INotifyPropertyChanged target,
                                            [CanBeNull] Func<bool> canExecute,
                                            [CanBeNull] Expression<Func<object>>[] dependentPropertyExpressions)
            : base(target, dependentPropertyExpressions)
        {
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return (Execution == null || Execution.IsCompleted)
                && (_canExecute == null || _canExecute());
        }

        public async Task<TResult> GetResultAsync(object parameter)
        {
            await ExecuteAsync(parameter);
            return Execution.Result;
        }

        public NotifyTaskCompletion<TResult> Execution
        {
            get { return _execution; }
            protected set
            {
                _execution = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
