using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace pestaServer.Controllers
{
    public class oauthcallbackController : Controller
    {
        //
        // GET: /oauthcallback/

        public ActionResult Index()
        {
            return View();
        }

    }
}
