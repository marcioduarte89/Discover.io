using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientAppOne.Services
{
    public class ClientTwoService : IClientTwoService
    {
        private readonly HttpClient _client;

        public ClientTwoService(HttpClient client)
        {
            _client = client;
        }

        public async Task<string[]> GetWeather()
        {
            var result = await _client.GetAsync("weatherforecast");

            var contentStream = await result.Content.ReadAsStreamAsync();

            return DeserializeJsonFromStream<string[]>(contentStream);
        }

        /// <summary>
        /// Deserialize from a stream instead of Deserialize to a string into memory and then into the object
        /// </summary>
        /// <typeparam name="TEntity">TEntity type</typeparam>
        /// <param name="stream">response stream</param>
        /// <returns>Returns an instance of the TEntity</returns>
        private TSource DeserializeJsonFromStream<TSource>(Stream stream)
        {
            if (stream == null || !stream.CanRead)
            {
                return default(TSource);
            }

            using (var sr = new StreamReader(stream))
            {
                using (var jtr = new JsonTextReader(sr))
                {
                    var js = new JsonSerializer();
                    var searchResult = js.Deserialize<TSource>(jtr);
                    return searchResult;
                }
            }
        }
    }

    public interface IClientTwoService
    {
        public Task<string[]> GetWeather();
    }
}
