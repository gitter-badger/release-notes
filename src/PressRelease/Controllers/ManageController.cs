using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using PressRelease.Models;
using PressRelease.ViewModels.Manage;

namespace PressRelease.Controllers
{
	[Authorize]
	public class ManageController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ILogger _logger;

		public ManageController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ILoggerFactory loggerFactory )
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = loggerFactory.CreateLogger<ManageController>();
		}

		//
		// GET: /Manage/Index
		[HttpGet]
		public async Task<IActionResult> Index( ManageMessageId? message = null )
		{
			ViewData["StatusMessage"] =
				message == ManageMessageId.Error
					? "An error has occurred."
					: message == ManageMessageId.RemovePhoneSuccess
						? "Your phone number was removed."
						: "";

			var user = await GetCurrentUserAsync();
			var model = new IndexViewModel
			{
				HasPassword = await _userManager.HasPasswordAsync( user ),
				Logins = await _userManager.GetLoginsAsync( user ),
				BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync( user )
			};
			return View( model );
		}

		//
		// POST: /Manage/RemoveLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RemoveLogin( RemoveLoginViewModel account )
		{
			ManageMessageId? message = ManageMessageId.Error;
			var user = await GetCurrentUserAsync();
			if ( user != null )
			{
				var result = await _userManager.RemoveLoginAsync( user, account.LoginProvider, account.ProviderKey );
				if ( result.Succeeded )
				{
					await _signInManager.SignInAsync( user, isPersistent: false );
					message = ManageMessageId.RemoveLoginSuccess;
				}
			}
			return RedirectToAction(
				nameof( ManageLogins ),
				new
				{
					Message = message
				} );
		}

		//GET: /Manage/ManageLogins
		[HttpGet]
		public async Task<IActionResult> ManageLogins( ManageMessageId? message = null )
		{
			ViewData["StatusMessage"] =
				message == ManageMessageId.RemoveLoginSuccess
					? "The external login was removed."
					: message == ManageMessageId.AddLoginSuccess
						? "The external login was added."
						: message == ManageMessageId.Error
							? "An error has occurred."
							: "";
			var user = await GetCurrentUserAsync();
			if ( user == null )
			{
				return View( "Error" );
			}
			var userLogins = await _userManager.GetLoginsAsync( user );
			var otherLogins = _signInManager.GetExternalAuthenticationSchemes().Where( auth => userLogins.All( ul => auth.AuthenticationScheme != ul.LoginProvider ) ).ToList();
			ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;
			return View(
				new ManageLoginsViewModel
				{
					CurrentLogins = userLogins,
					OtherLogins = otherLogins
				} );
		}

		//
		// POST: /Manage/LinkLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult LinkLogin( string provider )
		{
			// Request a redirect to the external login provider to link a login for the current user
			var redirectUrl = Url.Action( "LinkLoginCallback", "Manage" );
			var properties = _signInManager.ConfigureExternalAuthenticationProperties( provider, redirectUrl, User.GetUserId() );
			return new ChallengeResult( provider, properties );
		}

		//
		// GET: /Manage/LinkLoginCallback
		[HttpGet]
		public async Task<ActionResult> LinkLoginCallback()
		{
			var user = await GetCurrentUserAsync();
			if ( user == null )
			{
				return View( "Error" );
			}
			var info = await _signInManager.GetExternalLoginInfoAsync( User.GetUserId() );
			if ( info == null )
			{
				return RedirectToAction(
					nameof( ManageLogins ),
					new
					{
						Message = ManageMessageId.Error
					} );
			}
			var result = await _userManager.AddLoginAsync( user, info );
			var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
			return RedirectToAction(
				nameof( ManageLogins ),
				new
				{
					Message = message
				} );
		}

		#region Helpers

		private void AddErrors( IdentityResult result )
		{
			foreach ( var error in result.Errors )
			{
				ModelState.AddModelError( string.Empty, error.Description );
			}
		}

		public enum ManageMessageId
		{
			AddPhoneSuccess,
			AddLoginSuccess,
			ChangePasswordSuccess,
			SetTwoFactorSuccess,
			SetPasswordSuccess,
			RemoveLoginSuccess,
			RemovePhoneSuccess,
			Error
		}

		private async Task<ApplicationUser> GetCurrentUserAsync()
		{
			return await _userManager.FindByIdAsync( HttpContext.User.GetUserId() );
		}

		#endregion
	}
}
