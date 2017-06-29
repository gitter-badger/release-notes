using System.Security.Claims;
using System.Threading.Tasks;
using Octokit;

namespace ReleaseNotes.Services
{
    public interface IGitHubClientProvider
    {
        Task<IGitHubClient> GetClientAsync(ClaimsPrincipal user);
    }
}