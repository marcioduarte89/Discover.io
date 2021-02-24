using Polly;
using System;
using System.Net.Http;

namespace Discoverio.Client.Policies
{
    public interface IErrorPolicy
    {
        Func<HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> RegistrationRetryPolicy { get; }
    }
}
