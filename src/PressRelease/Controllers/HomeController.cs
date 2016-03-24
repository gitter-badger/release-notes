using System.Security.Claims;
using Microsoft.AspNet.Mvc;

namespace PressRelease.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			if ( User.IsSignedIn() )
			{
				return RedirectToAction( nameof( PressController.Index ), "Press" );
			}
			return View();
		}

		public IActionResult Error()
		{
			return View();
		}
	}
}
