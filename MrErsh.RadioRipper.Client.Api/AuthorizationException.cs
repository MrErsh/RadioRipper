using System;

namespace MrErsh.RadioRipper.Client.Api
{
    public sealed class AuthorizationException : Exception
    {
        public AuthorizationException(string message = null)
            : base(message ?? "Authorization error")
        {
        }
    }
}
