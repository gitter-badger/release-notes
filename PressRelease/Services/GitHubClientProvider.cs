using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Octokit;

namespace PressRelease.Services
{
    public class GitHubClientProvider<TUser> : IGitHubClientProvider where TUser : class
    {
        private readonly UserManager<TUser> _userManager;
        private readonly GitHubClientProviderOptions _options;

        public GitHubClientProvider(UserManager<TUser> userManager, IOptions<GitHubClientProviderOptions> options)
        {
            _userManager = userManager;
            _options = options.Value;
        }

        public async Task<IGitHubClient> GetClientAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            var token = await _userManager.GetAuthenticationTokenAsync(
                user,
                GitHubAuthenticationDefaults.AuthenticationScheme,
                "access_token");

            var gitHubClient = new GitHubClient(
                new ProductHeaderValue(_options.ApplicationName, _options.ApplicationVersion)
            );
            gitHubClient.Credentials = new Credentials(token);
            return gitHubClient;
        }
    }
}