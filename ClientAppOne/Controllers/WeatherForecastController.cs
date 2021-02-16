using ClientAppOne.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientAppOne.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IClientTwoService _clientTwoService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IClientTwoService clientTwoService)
        {
            _logger = logger;
            _clientTwoService = clientTwoService;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var weather = await _clientTwoService.GetWeather();
            return weather;
        }
    }
}
