﻿using Microsoft.AspNetCore.Mvc;

namespace Sample.Controllers {
    public class HomeController : Controller {

        [Route("")]
        public IActionResult Index() {
            return View();
        }

        [Route("SearchApi")]
        public IActionResult SearchApi() {
            return View();
        }

        [Route("GroupedList")]
        public IActionResult GroupedList() {
            return View();
        }

    }
}
