using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace pestaServer.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Pesta server: Refer to http://code.google.com/p/pesta for more information";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
