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
        [Route("")]
        public IActionResult Index()
        {
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
