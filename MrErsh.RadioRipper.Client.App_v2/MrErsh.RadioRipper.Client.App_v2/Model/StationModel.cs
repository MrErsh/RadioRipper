using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using MrErsh.RadioRipper.Model;

namespace MrErsh.RadioRipper.Client.Model
{
    public sealed class StationModel : ViewModelBase
    {
        [NotNull]
        public Station Station { get; init; }

        public bool IsChecked { get; set; }
    }
}
