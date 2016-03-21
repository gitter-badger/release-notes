using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
