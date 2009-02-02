using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Pesta.Engine.gadgets.servlet;
using HttpRequestWrapper = Pesta.Engine.gadgets.http.HttpRequestWrapper;
using HttpResponseWrapper = Pesta.Engine.gadgets.http.HttpResponseWrapper;

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
