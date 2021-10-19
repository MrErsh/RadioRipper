using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using MrErsh.RadioRipper.Client.Services;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Mvvm
{
    public class ObservableObject : ViewModelBase
    {
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                storage = value;
                return true;
            }

            return Set(propertyName, ref storage, value);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            RaisePropertyChanged(propertyName);
        }

        internal void OnPropertyChangedInternal(string propertyName) => OnPropertyChanged(propertyName);
    }

    public abstract class BaseViewModel : ViewModelBase, INavigable
    {
        private int _counter;

        IThreadingService thrService => ServiceContainer.Get<IThreadingService>();

        public bool IsLoading { get; set; } // TODO: internal set

        internal void IncrementIsLoading()
        {         
            //// TODO: пока так
            //Interlocked.Increment(ref _counter);
            //if (_counter != 0) // хм
            //{
            //    thrService.OnUiThread(() =>
            //    {
            //        IsLoading = true;
            //        RaisePropertyChanged(nameof(IsLoading));
            //    });
            //}
        }

        internal void DecrementIsLoading()
        {
            //Interlocked.Decrement(ref _counter);
            //if (_counter == 0)
            //{
            //    thrService.OnUiThread(() =>
            //    {
            //        IsLoading = false;
            //        RaisePropertyChanged(nameof(IsLoading));
            //    });
            //}
        }

        public virtual Task NavigatedTo() => Task.CompletedTask;

        public virtual Task NavigatedFrom() => Task.CompletedTask;
    }

    public class LoadingTracker : IDisposable
    {
        private BaseViewModel _viewModel;

        public LoadingTracker([NotNull] BaseViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.IncrementIsLoading();
        }

        public void Dispose()
        {
            _viewModel.DecrementIsLoading();
        }
    }
}
