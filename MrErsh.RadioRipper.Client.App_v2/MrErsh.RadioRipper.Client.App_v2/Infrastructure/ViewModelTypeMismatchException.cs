using JetBrains.Annotations;
using System;

namespace MrErsh.RadioRipper.Client.Services
{
    public class ViewModelTypeMismatchException<TExpected> : Exception
    {
        public ViewModelTypeMismatchException([CanBeNull] Type actualType)
            : base($"Type mismatch. Expected: {typeof(TExpected).Name}, actual: {actualType?.Name ?? "unknown"}.")
        {
        }
    }
}
