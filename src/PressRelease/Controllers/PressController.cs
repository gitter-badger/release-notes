using System.Linq;
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

		public PressController(
			IGitHubClient github )
		{
			_github = github;
		}

		public async Task<IActionResult> Index()
		{
			var repos = await _github.GetRepositoriesAsync( User.FindFirstValue( "access_token" ) );
			return View();
		}
	}
}