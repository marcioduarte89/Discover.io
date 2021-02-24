using Discoverio.Client.Exceptions;
using Grpc.Core;
using Polly;
using System;
using System.Net;
using System.Net.Http;

namespace Discoverio.Client.Policies
{
    public class ErrorPolicy : IErrorPolicy
    {
        public Func<HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> RegistrationRetryPolicy => (x) =>
        {
            return Policy.HandleResult<HttpResponseMessage>(r =>
            {
                return r.StatusCode == HttpStatusCode.OK && StatusManager.GetStatusCode(r) == StatusCode.NotFound;
            })
           .WaitAndRetryAsync(1,  (count) => TimeSpan.FromSeconds(1));
        };
    }
}
