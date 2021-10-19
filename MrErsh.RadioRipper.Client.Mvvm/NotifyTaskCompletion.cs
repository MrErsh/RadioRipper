using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Mvvm
{
    public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
    {
        #region Constructor
        public NotifyTaskCompletion([NotNull] Task<TResult> task)
        {
            Changed(nameof(IsInProgress));
            Task = task;
            if (!task.IsCompleted)
                TaskCompletion = WatchTaskAsync(task);
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public Task<TResult> Task { get; }
        public Task TaskCompletion { get; }
        public TaskStatus Status => Task.Status;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsNotCompleted => !Task.IsCompleted;
        public bool IsCompletedSuccessfully => Task.IsCompletedSuccessfully;
        public bool IsCanceled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public bool IsInProgress => Task != null
            && (Task.Status == TaskStatus.Running
                || Task.Status == TaskStatus.WaitingForActivation
                || Task.Status == TaskStatus.WaitingForChildrenToComplete
                || Task.Status == TaskStatus.WaitingToRun);
        public AggregateException Exception => Task.Exception;
        public Exception InnerException => Exception?.InnerException;
        public string ErrorMessage => InnerException?.Message;
        public TResult Result => Task.Status == TaskStatus.RanToCompletion
            ? Task.Result
            : default;
        #endregion

        #region Private methods
        private async Task WatchTaskAsync(Task task)
        {
            Changed(nameof(IsInProgress));
            try
            {
                await task;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            };
            if (PropertyChanged == null)
                return;

            Changed(nameof(Status), nameof(IsCompleted), nameof(IsNotCompleted), nameof(IsInProgress));
            if (task.IsCanceled)
                Changed(nameof(IsCanceled));
            else if(task.IsFaulted)
                Changed(nameof(IsFaulted), nameof(Exception), nameof(InnerException), nameof(ErrorMessage));
            else
                Changed(nameof(IsCompletedSuccessfully), nameof(Result));

            Changed(nameof(IsInProgress));
        }

        private void Changed(params string[] props)
        {
            foreach (var p in props)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p)); // PropertyChanged != null
        }
        #endregion
    }
}
