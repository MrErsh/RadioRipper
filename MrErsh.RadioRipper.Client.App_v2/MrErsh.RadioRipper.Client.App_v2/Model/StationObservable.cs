using GalaSoft.MvvmLight;
using MrErsh.RadioRipper.Model;

namespace MrErsh.RadioRipper.Client.Model
{
    //[PropertyChanged.AddINotifyPropertyChangedInterface]
    //public sealed class StationObservable
    public sealed class StationObservable : ObservableObject
    {
        public Station Station { get; }

        public StationObservable(Station station) => Station = station;

        public StationObservable() : this(new Station()) { }

        public string Url
        { 
            get => Station.Url; 
            set => Station.Url = value;
        }

        public string Name
        { 
            get => Station.Name;
            set => Station.Name = value;
        }
    }
}
