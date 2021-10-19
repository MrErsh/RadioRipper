using Microsoft.Extensions.Logging;

namespace MrErsh.RadioRipper.WebApi.Logging
{
    public static class Events
    {
        public static class Errors
        {
            public static EventId ParseError => new(1000, "StreamParseError");
            public static EventId StartTrackingError => new(1001, "StartTrackingError");
        }

        public static class Warnings
        {
            public static EventId AuthenticationWarn => new(2000, "AuthenticationWarn");
        }
    }
}
