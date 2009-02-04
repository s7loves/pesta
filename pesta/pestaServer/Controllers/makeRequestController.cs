using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using pestaServer.Models.gadgets.servlet;
using HttpRequestWrapper = pestaServer.Models.gadgets.http.HttpRequestWrapper;
using HttpResponseWrapper = pestaServer.Models.gadgets.http.HttpResponseWrapper;

namespace pestaServer.Controllers
{
    public class makeRequestController : Controller
    {
        private static readonly MakeRequestHandler makeRequestHandler = MakeRequestHandler.Instance;

        public void Index()
        {
            HttpRequestWrapper request = new HttpRequestWrapper(System.Web.HttpContext.Current);
            HttpResponseWrapper response = new HttpResponseWrapper(System.Web.HttpContext.Current.Response);
            try
            {
                makeRequestHandler.fetch(request, response);
            }
            catch (Exception e)
            {
                outputError(e, response.getResponse());
            }
        }

        private static void outputError(Exception e, HttpResponse resp)
        {
            resp.StatusCode = (int)HttpStatusCode.BadRequest;
            resp.StatusDescription = e.Message;
        }
    }
}
