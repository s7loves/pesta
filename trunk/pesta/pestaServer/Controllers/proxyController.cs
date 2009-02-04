using System;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using pestaServer.Models.gadgets.servlet;
using HttpResponseWrapper = pestaServer.Models.gadgets.http.HttpResponseWrapper;

namespace pestaServer.Controllers
{
    public class proxyController : Controller
    {
        private ProxyHandler proxyHandler = ProxyHandler.Instance;

        public void Index()
        {
            HttpResponse response = System.Web.HttpContext.Current.Response;
            HttpResponseWrapper wrapper = new HttpResponseWrapper(response);
            try
            {
                proxyHandler.fetch(new ProxyRequestWrapper(System.Web.HttpContext.Current), wrapper);
            }
            catch (Exception ex)
            {
                outputError(ex, response);
                return;
            }
            response.End();
        }
        private void outputError(Exception excep, HttpResponse resp)
        {
            StringBuilder err = new StringBuilder();
            err.Append(excep.Source);
            err.Append(" proxy(");
            err.Append(") ");
            err.Append(excep.Message);

            // Log the errors here for now. We might want different severity levels
            // for different error codes.
            resp.StatusCode = (int)HttpStatusCode.BadRequest;
            resp.StatusDescription = err.ToString();
        }
    }
}
