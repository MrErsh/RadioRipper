using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Mvvm
{
    public abstract class AsyncCommandBase : IAsyncCommand
    {
        protected HashSet<string> DependentPropertyNames { get; private set; }

        protected AsyncCommandBase([CanBeNull] INotifyPropertyChanged target = null, [CanBeNull] Expression<Func<object>>[] dependentPropertyExpressions = null)
        {
            if (target == null)
                return;

            DependentPropertyNames = DependentCommandHelper.CreateProps(dependentPropertyExpressions);
            target.PropertyChanged += TargetPropertyChanged;
        }

        #region ICommand
        public abstract bool CanExecute(object parameter);
        public event EventHandler CanExecuteChanged;
        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter).ConfigureAwait(false);
        }
        #endregion

        #region IAsyncCommand
        public abstract Task ExecuteAsync(object parameter);
        #endregion

        #region IHasLabel
        public string Label { get; set; }
        #endregion

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
     
        private void TargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (DependentPropertyNames.Contains(e.PropertyName))
            {
                RaiseCanExecuteChanged();
            }
        }
    }
}
