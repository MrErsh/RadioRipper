using MrErsh.RadioRipper.Core;
using MrErsh.RadioRipper.Model;
using System;

namespace MrErsh.RipperDemo
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var server =
                "http://listen.181fm.com/181-goodtime_128k.mp3?aw_0_1st.playerid=esPlayer&aw_0_1st.skey=1616135327";

            var station = new Station() { Id = Guid.NewGuid(), Url = server };
            var ripper = new Ripper(null);
            var timered = new TimeredRadioRipper(ripper, station, null);
            var settings = new RipperSettings(1000 * 5, 10);
            timered.TrackChanged += Timered_TrackChanged;
            timered.Run(settings);

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }

        private static void Timered_TrackChanged(object sender, TrackChangedEventArg e)
        {
            var title = e.Info?.StreamTitle;
            Console.WriteLine(title);
        }
    }
}
