using MrErsh.RadioRipper.Dal;
using MrErsh.RadioRipper.IdentityDal;
using MrErsh.RadioRipper.Model;
using System.Collections.Generic;

namespace MrErsh.RadioRipper.WebApi
{
    public class DemoUserDbInitializer
    {
        private readonly Dictionary<string, string> _stations = new()
        {
            { "MEGATON CAFE RADIO", @"http://us2.internet-radio.com:8443/" },
            { "Oldies \"S\" ('60s '70s '80s)", @"http://de1.internet-radio.com:8204/" },
            { "Nu-jazz", @"http://79.111.14.76:8000/nujazz" }
        };

        public void AddDefaultStations(RadioDbContext context, string userName = SeedData.DemoUserId)
        {
            foreach(var st in _stations)
            {
                context.Stations.Add(new Station { Name = st.Key, Url = st.Value, UserId = userName });
            }
        }
    }
}
