using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace MrErsh.RadioRipper.Client.Mvvm
{
    public static class DependentCommandExtensions
    {
        public static DependentCommand DependentCommand(this INotifyPropertyChanged target, Action execute, Func<bool> canExecute, params Expression<Func<object>>[] dependentPropertyExpressions)
            => new DependentCommand(target, execute, canExecute, dependentPropertyExpressions);
    }

    public class DependentCommand : RelayCommand, IHasLabel
    {
        private readonly List<string> _dependentPropertyNames;

        public DependentCommand(INotifyPropertyChanged target, Action execute, Func<bool> canExecute, params Expression<Func<object>>[] dependentPropertyExpressions)
            : base(execute, canExecute, true) // keepTargetAlive!
        {
            _dependentPropertyNames = new List<string>();
            foreach (var body in dependentPropertyExpressions.Select(expression => expression.Body))
            {
                var expression = body as MemberExpression;
                if (expression != null)
                {
                    _dependentPropertyNames.Add(expression.Member.Name);
                }
                else
                {
                    var unaryExpression = body as UnaryExpression;
                    if (unaryExpression != null)
                    {
                        _dependentPropertyNames.Add(((MemberExpression)unaryExpression.Operand).Member.Name);
                    }
                }
            }

            target.PropertyChanged += TargetPropertyChanged;
        }

        public string Label { get; init; }

        private void TargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_dependentPropertyNames.Contains(e.PropertyName))
            {
                RaiseCanExecuteChanged();
            }
        }
    }
}
