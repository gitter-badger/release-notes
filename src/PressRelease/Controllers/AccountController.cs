using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using PressRelease.Models;
using Microsoft.AspNet.Authentication.Cookies;

namespace PressRelease.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ILogger _logger;

		public AccountController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ILoggerFactory loggerFactory )
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = loggerFactory.CreateLogger<AccountController>();
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LogOff()
		{
			await _signInManager.SignOutAsync();
			_logger.LogInformation( 4, "User logged out." );
			return RedirectToAction( nameof( HomeController.Index ), "Home" );
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public IActionResult Login()
		{
			// Request a redirect to the external login provider.
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(
				GitHubAuthenticationDefaults.AuthenticationScheme,
				Url.Action( "ExternalLoginCallback", "Account" ) );
			return new ChallengeResult( GitHubAuthenticationDefaults.AuthenticationScheme, properties );
		}

		//
		// GET: /Account/ExternalLoginCallback
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallback()
		{
			var redirect = RedirectToAction( nameof( HomeController.Index ), "Home" );
			var info = await _signInManager.GetExternalLoginInfoAsync();
			if ( info == null ) return redirect;

			var user = await _userManager.FindByLoginAsync( info.LoginProvider, info.ProviderKey );
			if ( user != null )
			{
				var claims = await _userManager.GetClaimsAsync( user );
				await _userManager.ReplaceClaimAsync( user, claims.Single( c => c.Type == "access_token" ), info.ExternalPrincipal.FindFirst( "access_token" ) );
			}

			// Sign in the user with this external login provider if the user already has a login.
			var result = await _signInManager.ExternalLoginSignInAsync( info.LoginProvider, info.ProviderKey, isPersistent: true );
			if ( result.Succeeded )
			{
				_logger.LogInformation( 5, "User logged in with {Name} provider.", info.LoginProvider );
			}
			else
			{
				// If the user does not have an account, then create an account.
				var newUser = await CreateUserFromLogin( info );
				if ( newUser != null )
				{
					await _signInManager.SignInAsync( newUser, isPersistent: true, authenticationMethod: info.LoginProvider );
					_logger.LogInformation( 6, "User created an account using {Name} provider.", info.LoginProvider );
				}
			}
			return redirect;
		}

		private async Task<ApplicationUser> CreateUserFromLogin( ExternalLoginInfo info )
		{
			var email = info.ExternalPrincipal.FindFirstValue( ClaimTypes.Email );
			var newUser = new ApplicationUser
			{
				UserName = email,
				Email = email
			};

			var result = await _userManager.CreateAsync( newUser );
			if ( !result.Succeeded )
			{
				AddErrors( result );
				return null;
			}

			result = await _userManager.AddLoginAsync( newUser, info );
			if ( !result.Succeeded )
			{
				AddErrors( result );
				return null;
			}
			result = await _userManager.AddClaimAsync( newUser, info.ExternalPrincipal.FindFirst( "access_token" ) );
			if ( !result.Succeeded )
			{
				AddErrors( result );
				return null;
			}

			return newUser;
		}

		private void AddErrors( IdentityResult result )
		{
			foreach ( var error in result.Errors )
			{
				ModelState.AddModelError( string.Empty, error.Description );
			}
		}
	}
}
