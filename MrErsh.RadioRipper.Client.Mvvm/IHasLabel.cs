using JetBrains.Annotations;

namespace MrErsh.RadioRipper.Client.Mvvm
{
    public interface IHasLabel
    {
        [CanBeNull]
        public string Label { get; }
    }
}
