using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using PressRelease.Models;
using PressRelease.Services;

namespace PressRelease.Controllers
{
	[Authorize]
	public class PressController : Controller
	{
		private readonly IGitHubClient _github;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public PressController(
			IGitHubClient github,
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager )
		{
			_github = github;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<IActionResult> Index()
		{
			try
			{

				var repos = await _github.GetRepositoriesAsync( User.FindFirstValue( "access_token" ) );
				return View();
			}
			catch
			{
				await _signInManager.SignOutAsync();
				return RedirectToAction( nameof( HomeController.Index ), "Home" );
			}
		}
	}
}