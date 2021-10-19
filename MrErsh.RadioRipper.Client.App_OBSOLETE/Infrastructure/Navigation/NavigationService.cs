using JetBrains.Annotations;
using MrErsh.RadioRipper.Client.Shared.Mvvm;
using System;

namespace MrErsh.RadioRipper.Client.Infrastructure
{
    public interface INavigable
    {
        void OnNavigating(object param);
    }

    public class NavigableViewModel<TParameter> : BaseViewModel, INavigable where TParameter : class
    {
        public void OnNavigating([CanBeNull]object param)
        {
            var typedParam = param as TParameter;
            OnNavigatingInternal(typedParam);
        }

        protected virtual void OnNavigatingInternal([CanBeNull] TParameter param) { }
    }

    public interface INavigationService
    {
        public void NavigateTo(Type page, object parameter = null);

        public void GoBack();
    }

    public class SimpleNavigationService : INavigationService
    {
        public void GoBack()
        {
            // TODO VE:
            throw new NotImplementedException();
        }

        public void NavigateTo(Type page, object parameter = null)
        {
            throw new NotImplementedException();
        }
    }
}
