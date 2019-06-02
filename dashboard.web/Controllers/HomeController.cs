using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dashboard.web.Models;
using WebPush;

namespace dashboard.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRemindersProvider _remindersProvider;

        public HomeController(IRemindersProvider remindersProvider)
        {
            _remindersProvider = remindersProvider;
        }

        public IActionResult Index()
        {
            return View(new HomepageViewModel(_remindersProvider.Get()));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
