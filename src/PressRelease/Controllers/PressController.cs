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
		private readonly SignInManager<ApplicationUser> _signInManager;

		public PressController(
			SignInManager<ApplicationUser> signInManager )
		{
			_signInManager = signInManager;
		}

		public async Task<IActionResult> Index()
		{
			var x = new GitHubServices( User.FindFirstValue( "access_token" ) );
			var user = await x.GetUserAsync();
			return View();
		}
	}
}