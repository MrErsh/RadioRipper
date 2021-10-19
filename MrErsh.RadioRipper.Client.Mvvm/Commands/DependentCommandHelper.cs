using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MrErsh.RadioRipper.Client.Mvvm
{
    internal static class DependentCommandHelper
    {
        [NotNull]
        public static HashSet<string> CreateProps([CanBeNull] params Expression<Func<object>>[] dependentPropertyExpressions)
        {
            var props = new HashSet<string>();
            if (dependentPropertyExpressions == null)
                return props;

            foreach (var body in dependentPropertyExpressions.Select(expression => expression.Body))
            {
                var expression = body as MemberExpression;
                if (expression != null)
                {
                    props.Add(expression.Member.Name);
                }
                else
                {
                    var unaryExpression = body as UnaryExpression;
                    if (unaryExpression != null)
                    {
                        props.Add(((MemberExpression)unaryExpression.Operand).Member.Name);
                    }
                }
            }

            return props;
        }
    }
}
