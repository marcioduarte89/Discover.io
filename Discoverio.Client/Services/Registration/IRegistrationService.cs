using DiscoveryService.Services;
using System.Threading.Tasks;

namespace Discoverio.Client.Services.Registration
{
    public interface IRegistrationService
    {
        Task<RegistrationStatus> Register();
    }
}
