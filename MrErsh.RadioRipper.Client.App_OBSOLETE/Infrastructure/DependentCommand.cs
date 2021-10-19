using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;

namespace MrErsh.RadioRipper.Client.Infrastructure
{
    public static class DependentCommandExtensions
    {
        public static DependentDelegateCommand DependentCommand(this INotifyPropertyChanged target, Action execute, Func<bool> canExecute, params Expression<Func<object>>[] dependentPropertyExpressions)
            => new DependentDelegateCommand(execute, canExecute, target, dependentPropertyExpressions);
    }

    public class DependentDelegateCommand : RelayCommand
    {
        private readonly List<string> _dependentPropertyNames;

        // TODO VE: Слишком монструозно
        public DependentDelegateCommand(Action execute, Func<bool> canExecute, INotifyPropertyChanged target, params Expression<Func<object>>[] dependentPropertyExpressions)
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

        private void TargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_dependentPropertyNames.Contains(e.PropertyName))
            {
                RaiseCanExecuteChanged();
            }
        }
    }
}
