using System.Security.Claims;
using System.Threading.Tasks;
using Octokit;

namespace PressRelease.Services
{
    public interface IGitHubClientProvider
    {
        Task<IGitHubClient> GetClientAsync(ClaimsPrincipal user);
    }
}