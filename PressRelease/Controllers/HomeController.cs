using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using PressRelease.Models;
using AspNet.Security.OAuth.GitHub;

namespace PressRelease.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGitHubClient _github;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IGitHubClient github)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _github = github;
        }

        [Route("")]
        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                var token = await _userManager.GetAuthenticationTokenAsync(
                    user,
                 GitHubAuthenticationDefaults.AuthenticationScheme,
                  "access_token");
            }
            return View();
        }

        [Route("About")]
        public IActionResult About()
        {
            return View();
        }

        [Route("Error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
