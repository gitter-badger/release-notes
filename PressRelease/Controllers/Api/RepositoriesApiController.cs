using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using PressRelease.Models;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authorization;
using PressRelease.Services;
using PressRelease.Models.Api;

namespace PressRelease.Controllers
{
    [Route("/api/repository")]
    [Authorize]
    // [ValidateAntiForgeryToken]
    public class RepositoriesApiController : Controller
    {
        private readonly IGitHubClientProvider _githubClientProvider;

        public RepositoriesApiController(IGitHubClientProvider githubClientProvider)
        {
            _githubClientProvider = githubClientProvider;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetRepositories()
        {
            var githubClient = await _githubClientProvider.GetClientAsync(User);
            var requestOptions = new RepositoryRequest
            {
                Sort = RepositorySort.FullName,
                Direction = SortDirection.Ascending,
                Visibility = RepositoryVisibility.All
            };
            var repositories = await githubClient.Repository.GetAllForCurrent(requestOptions);
            var results = repositories.Select(r => new RepositoryModel { Name = r.FullName, Description = r.Description });
            return Json(results);
        }
    }
}
