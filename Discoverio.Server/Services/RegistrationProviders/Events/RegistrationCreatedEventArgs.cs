using System;

namespace Discoverio.Server.Services.RegistrationProviders.Events
{
    public class RegistrationCreatedEventArgs : EventArgs
    {
        public Registration Registration { get; set; }
    }
}
