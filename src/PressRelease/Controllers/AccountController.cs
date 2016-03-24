using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using PressRelease.Models;

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
		public IActionResult Login( string returnUrl = null )
		{
			// Request a redirect to the external login provider.
			var redirectUrl = Url.Action( "ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl } );
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(
				GitHubAuthenticationDefaults.AuthenticationScheme,
				redirectUrl );
			return new ChallengeResult( GitHubAuthenticationDefaults.AuthenticationScheme, properties );
		}

		//
		// GET: /Account/ExternalLoginCallback
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallback( string returnUrl = null )
		{
			var info = await _signInManager.GetExternalLoginInfoAsync();
			if ( info == null )
			{
				return RedirectToAction( nameof( HomeController.Index ), "Home" );
			}


			var user = await _userManager.FindByLoginAsync( info.LoginProvider, info.ProviderKey );
			if ( user != null )
			{
				var claims = await _userManager.GetClaimsAsync( user );
				await _userManager.ReplaceClaimAsync( user, claims.Single( c => c.Type == "access_token" ), info.ExternalPrincipal.FindFirst( "access_token" ) );
			}

			// Sign in the user with this external login provider if the user already has a login.
			var result = await _signInManager.ExternalLoginSignInAsync( info.LoginProvider, info.ProviderKey, isPersistent: false );
			if ( result.Succeeded )
			{
				_logger.LogInformation( 5, "User logged in with {Name} provider.", info.LoginProvider );
				return RedirectToLocal( returnUrl );
			}

			// If the user does not have an account, then create an account.
			ViewData["ReturnUrl"] = returnUrl;
			var email = info.ExternalPrincipal.FindFirstValue( ClaimTypes.Email );

			var newUser = new ApplicationUser
			{
				UserName = email,
				Email = email
			};
			var newUserResult = await _userManager.CreateAsync( newUser );
			if ( newUserResult.Succeeded )
			{
				newUserResult = await _userManager.AddLoginAsync( newUser, info );
				if ( newUserResult.Succeeded )
				{
					await _userManager.AddClaimAsync( newUser, info.ExternalPrincipal.FindFirst( "access_token" ) );
					await _signInManager.SignInAsync( newUser, isPersistent: false );
					_logger.LogInformation( 6, "User created an account using {Name} provider.", info.LoginProvider );
					return RedirectToLocal( returnUrl );
				}
			}
			AddErrors( newUserResult );
			return View( "ExternalLoginFailure" );
		}

		#region Helpers

		private void AddErrors( IdentityResult result )
		{
			foreach ( var error in result.Errors )
			{
				ModelState.AddModelError( string.Empty, error.Description );
			}
		}

		private async Task<ApplicationUser> GetCurrentUserAsync()
		{
			return await _userManager.FindByIdAsync( HttpContext.User.GetUserId() );
		}

		private IActionResult RedirectToLocal( string returnUrl )
		{
			if ( Url.IsLocalUrl( returnUrl ) )
			{
				return Redirect( returnUrl );
			}
			else
			{
				return RedirectToAction( nameof( HomeController.Index ), "Home" );
			}
		}

		#endregion
	}
}
