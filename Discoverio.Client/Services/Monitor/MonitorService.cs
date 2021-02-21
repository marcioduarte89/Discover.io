using DiscoveryService.Services;
using System.Threading.Tasks;
using static DiscoveryService.Services.MonitorService;

namespace Discoverio.Client.Services.Monitor
{
    public class MonitorService : IMonitorService
    {
        private readonly MonitorServiceClient _monitorServiceClient;

        public MonitorService(MonitorServiceClient monitorServiceClient)
        {
            _monitorServiceClient = monitorServiceClient;
        }

        public async Task<bool> SendHeartBeat(UUID uniqueId)
        {
            return (await _monitorServiceClient.HeartBeatAsync(uniqueId)).Success;
        }
    }
}
