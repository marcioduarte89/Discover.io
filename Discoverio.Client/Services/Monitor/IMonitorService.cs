using DiscoveryService.Services;
using System.Threading.Tasks;

namespace Discoverio.Client.Services.Monitor
{
    public interface IMonitorService
    {
        Task<bool> SendHeartBeat(UUID uniqueId);
    }
}
