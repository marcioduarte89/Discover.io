using System;

namespace Discoverio.Server.Services.RegistrationProviders.Events
{
    public class RegistrationExpiredEventArgs : EventArgs
    {
        public Registration Registration { get; set; }
    }
}
